//Get port and validate it
var port = parseInt( process.argv[2] );
if( isNaN( port ) ) {
    console.error( "ERROR: No port (or invalid port) specified." );
    process.exit( 1 );
}




/**************\
|*  INCLUDES  *|
\**************/
//node.js
var http     = require( "http" );
var url      = require( "url" );
var fs       = require( "fs" );

//third party
var mysql2   = require( "mysql2" );
var strftime = require( "strftime" );

//project
var Connection = require( "./connection" );
var log        = require( "./log" );




/**************\
|* CONSTANTS  *|
\**************/
var logfile             = "log.txt";
var configfile          = "config.json";
var ipv6_is_ipv4_prefix = "::ffff:";
var FLAG_RECOMMENDED    = 1;




/**************\
|*  GLOBALS   *|
\**************/
var routes = [];
var mysql_params = {
    "user":     null,
    "password": null,
    "database": null
};




/**************\
|* FUNCTIONS  *|
\**************/
/**
 * If "addr" is an IPv6 address specifying an IPv4 address,
 * the IPv4 portion of the address is returned. Otherwise, the given address is returned.
 * Example:
 *     "::ffff:127.0.0.1" becomes "127.0.0.1"
 *     "::1" stays "::1"
 * NOTE: Currently only IPv6 address strings beginning with "::ffff:" are recognized.
 * @param {String} addr - The address
 * @returns {String}
 */
function trim_ip( addr ) {
    if( addr.substr( 0, ipv6_is_ipv4_prefix.length ) == ipv6_is_ipv4_prefix )
        return addr.substr( ipv6_is_ipv4_prefix.length );
    return addr;
}

/**
 * Logs details of a request made to the server.
 * @param {http.IncomingMessage} req - The request
 */
function log_request( req ) {
    //Log a detailed message including:
    var useragent = req.headers["user-agent"];
    log.out(
        "[" + strftime( "%d-%b-%yT%H:%M:%S" ) + "] "  + //The date & time the request was made
        trim_ip( req.socket.remoteAddress )           + //The user's IP
        " "                                           +
        ( useragent ? "(" + useragent + ") " :  "" )  + //The User-Agent string (if available)
        req.method                                    + //Request method (GET, POST, etc)
        " "                                           +
        req.url                                         //Request URL
    );
}

/**
 * Add a route the server can handle.
 * @param {Function} callback - Callback function.
 *     Callbacks take as arguments:
 *         {Connection} conn - The connection whose route was matched.
 *         {String} match1, match2, ... - If the regular expression for the route includes parentheses, these store capture 1, 2, and so on.
 * @param {RegExp} regex - Regular expression describing a route.
 */
function add_route( callback, regex ) {
    routes.push( {
        "callback": callback,
        "regex":    regex
    } );
}

/**
 * Given a connection, try to find a route that can satisfy the requested URL.
 * @param {Connection} conn - The connection to be routed.
 * @returns {Boolean} true if the URL matched a route, false otherwise
 */
function try_routes( conn ) {
    //Note: using .some() instead of .forEach() here
    //because it needs to terminate once a route has been found.
    //In the function below, return true acts as a "break" statement,
    //whereas return false acts as "continue"
    return routes.some( function( route ) {
        var result = route.regex.exec( conn.url.pathname );
        if( result == null )
            return false;

        route.callback.apply( null, [conn].concat( result.slice( 1 ) ) );
        return true;
    } );
}




/**************\
|*   ROUTES   *|
\**************/

//Returns /robots.txt
//Robots are instructed not to index anything the API returns.
var robots_headers = { "Content-Type": "text/plain" };
add_route( function( conn ) {
    conn.end(
        "User-agent: *\n" +
        "Disallow: /\n",
        200,
        robots_headers
    );
}, /^\/robots.txt$/);

//List of all packages.
//Examples: "/packages" or "/packages/"
add_route( function( conn ) {
    conn.sql.query(
        "SELECT name, last_update as lastUpdated FROM Package",
        function( err, rows, fields ) {
            if( err )
                return conn.mysql_error( err );

            conn.end( JSON.stringify( rows ) );
        }
    );
}, /^\/packages\/?$/ );

//Describe a specific package.
//Examples: "/packages/forge" or "/packages/forge/"
add_route( function( conn, package_name ) {
    var pkg;
    var package_id;
    var builds_by_id = {};
    var build_ids;

    var count = 0;

    function grab_name() {
        //Attempt to find the id and official name of a package with the given name.
        //Note: Prepared statements will automatically .escape() provided parameters.
        conn.sql.query(
            "SELECT id, name "    +
                "FROM Package "   +
                "WHERE name = ? " +
                "LIMIT 1",
            [ package_name ],
            function( err, rows, fields ) {
                if( err )
                    return conn.mysql_error( err );
                if( rows.length == 0 )
                    return conn.error( "Package not found.", 404 );

                //This package exists. Grab the official name & id.
                pkg = {
                    "name":    rows[0].name,
                    "authors": null,
                    "builds":  null
                }
                packageid = rows[0].id;

                grab_authors();
                grab_builds();
            }
        );
    }

    //Get authors
    function grab_authors() {
        conn.sql.query(
            "SELECT author "          +
                "FROM PackageAuthor " +
                "WHERE packageid = ?",
            [ packageid ],
            function( err, rows, fields ) {
                if( err )
                    return conn.mysql_error( err );

                pkg.authors = rows.map( function(x) { return x.author } );
                finish();
            }
        );
    }

    //Get builds
    function grab_builds() {
        conn.sql.query(
            "SELECT id, ver, given, flags " +
                "FROM Build "               +
                "WHERE packageid = ?",
            [ packageid ],
            function( err, rows, fields ) {
                if( err )
                    return conn.mysql_error( err );

                pkg.builds = [];
                rows.forEach( function( x ) {
                    var build = {
                        "version":      x.ver,
                        "givenVersion": x.given,
                        "recommended":  Boolean( x.flags & FLAG_RECOMMENDED ),
                        "interfaces":   [],
                        "dependencies": {
                            "packages":   [],
                            "interfaces": []
                        },
                        "conflicts":    [],
                        "hashes":       []
                    };
                    pkg.builds.push( build );
                    builds_by_id[ x.id ] = build;
                } );
                build_ids = Object.keys( builds_by_id );

                grab_interfaces();
                grab_package_dependencies();
                grab_interface_dependencies();
                grab_conflicts();
                grab_hashes();
            }
        );
    }

    //Interfaces
    function grab_interfaces() {
        conn.sql.query(
            "SELECT biv.buildid, i.name, iv.ver "           +
                "FROM       BuildInterfaceVersion AS biv "  +
                "INNER JOIN InterfaceVersion      AS iv "   +
                    "ON biv.ifverid = iv.id "               +
                "INNER JOIN Interface             AS i "    +
                    "ON iv.interfaceid = i.id "             +
                "WHERE biv.buildid IN (?)",
            [ build_ids ],
            function( err, rows, fields ) {
                if( err )
                    return conn.mysql_error( err );

                rows.forEach( function( x ) {
                    builds_by_id[ x.buildid ].interfaces.push( {
                        "name": x.name, "version": x.ver
                    } );
                } );
                finish();
            }
        );
    }

    function grab_package_dependencies() {
        conn.sql.query(
            "SELECT pd.buildid, p.name, pd.version_range AS version " +
            "FROM       PackageDependency AS pd "                     +
            "INNER JOIN Package           AS p "                      +
                "ON pd.packageid = p.id "                             +
            "WHERE pd.buildid IN (?)",
            [ build_ids ],
            function( err, rows, fields ) {
                if( err )
                    return conn.mysql_error( err );

                rows.forEach( function( x ) {
                    builds_by_id[ x.buildid ].dependencies.packages.push( {
                        "name": x.name, "version": x.version
                    } );
                } );
                finish();
            }
        );
    }

    function grab_interface_dependencies() {
        conn.sql.query(
            "SELECT idp.buildid, i.name, idp.version_range AS version " +
            "FROM       InterfaceDependency AS idp "                    +
            "INNER JOIN Interface           AS i "                      +
                "ON idp.interfaceid = i.id "                            +
            "WHERE idp.buildid IN (?)",
            [ build_ids ],
            function( err, rows, fields ) {
                if( err )
                    return conn.mysql_error( err );

                rows.forEach( function( x ) {
                    builds_by_id[ x.buildid ].dependencies.interfaces.push( {
                        "name": x.name, "version": x.version
                    } );
                } );
                finish();
            }
        );
    }

    function grab_conflicts() {
        //STUB
        finish();
    }

    function grab_hashes() {
        conn.sql.query(
            "SELECT buildid, HEX( hash ) AS hash " +
            "FROM BuildHash "                      +
            "WHERE buildid IN (?) "                +
            "ORDER BY hashid ASC",
            [ build_ids ],
            function( err, rows, fields ) {
                if( err )
                    return conn.mysql_error( err );

                rows.forEach( function( x ) {
                    builds_by_id[ x.buildid ].hashes.push( x.hash );
                } );
                finish();
            }
        );
    }

    //Called when finished
    function finish() {
        //The following functions must complete before we can output the data:
        //grab_authors()
        //grab_interfaces()
        //grab_package_dependencies()
        //grab_interface_dependencies()
        //grab_conflicts()
        //grab_hashes()
        if( ++count != 6 )
            return;

        //Finished; output package JSON
        conn.end( JSON.stringify( pkg ) );
    }

    grab_name();
}, /^\/packages\/([^/]+)\/?$/ );

//Describe a specific version of a package.
//Examples: "/packages/forge/1.2.3" or "/packages/forge/1.2.3/"
add_route( function( conn, package_name, version ) {
    var build;
    var build_id;
    var count = 0;

    function grab_name() {
        //Attempt to find the id and official name of a package with the given name.
        //Note: Prepared statements will automatically .escape() provided parameters.
        conn.sql.query(
            "SELECT b.id, b.ver, b.given, b.flags " +
                "FROM Build b "                     +
                "INNER JOIN Package p "             +
                    "ON b.packageid = p.id "        +
                "WHERE p.name = ? AND b.ver = ? "   +
                "LIMIT 1",
            [ package_name, version ],
            function( err, rows, fields ) {
                if( err )
                    return conn.mysql_error( err );
                if( rows.length == 0 )
                    return conn.error( "Package or given version of package not found.", 404 );

                //This package and the given version of this package exist. Grab the official version and id.
                var row = rows[0];
                build_id = row.id;
                build = {
                    "version":      row.ver,
                    "givenVersion": row.given,
                    "recommended":  Boolean( row.flags & FLAG_RECOMMENDED ),
                    "interfaces":   null,
                    "dependencies": {
                        "packages":   null,
                        "interfaces": null
                    },
                    "conflicts":    null,
                    "hashes":       null
                };

                grab_interfaces();
                grab_package_dependencies();
                grab_interface_dependencies();
                grab_conflicts();
                grab_hashes();
            }
        );
    }

    function grab_interfaces() {
        conn.sql.query(
            "SELECT i.name, iv.ver AS version "             +
                "FROM       BuildInterfaceVersion AS biv "  +
                "INNER JOIN InterfaceVersion      AS iv "   +
                    "ON biv.ifverid = iv.id "               +
                "INNER JOIN Interface             AS i "    +
                    "ON iv.interfaceid = i.id "             +
                "WHERE biv.buildid = ?",
            [ build_id ],
            function( err, rows, fields ) {
                if( err )
                    return conn.mysql_error( err );

                build.interfaces = rows;
                finish();
            }
        );
    }

    function grab_package_dependencies() {
        conn.sql.query(
            "SELECT p.name, pd.version_range AS version " +
            "FROM       PackageDependency AS pd "         +
            "INNER JOIN Package           AS p "          +
                "ON pd.packageid = p.id "                 +
            "WHERE pd.buildid = ?",
            [ build_id ],
            function( err, rows, fields ) {
                if( err )
                    return conn.mysql_error( err );

                build.dependencies.packages = rows;
                finish();
            }
        );
    }

    function grab_interface_dependencies() {
        conn.sql.query(
            "SELECT i.name, idp.version_range AS version " +
            "FROM       InterfaceDependency AS idp "       +
            "INNER JOIN Interface           AS i "         +
                "ON idp.interfaceid = i.id "               +
            "WHERE idp.buildid = ?",
            [ build_id ],
            function( err, rows, fields ) {
                if( err )
                    return conn.mysql_error( err );

                build.dependencies.interfaces = rows;
                finish();
            }
        );
    }

    function grab_conflicts() {
        //STUB
        build.conflicts = [];
        finish();
    }

    function grab_hashes() {
        conn.sql.query(
            "SELECT HEX( hash ) AS hash " +
            "FROM BuildHash "             +
            "WHERE buildid = ? "          +
            "ORDER BY hashid ASC",
            [ build_id ],
            function( err, rows, fields ) {
                if( err )
                    return conn.mysql_error( err );

                build.hashes = rows.map( function( x ) {
                    return x.hash;
                } );
                finish();
            }
        );
    }

    function finish() {
        //The following functions must complete before we can output the data:
        //grab_interfaces()
        //grab_package_dependencies()
        //grab_interface_dependencies()
        //grab_conflicts()
        //grab_hashes()
        if(  ++count != 5 )
            return;

        conn.end( JSON.stringify( build ) );
    }

    grab_name();
}, /^\/packages\/([^/]+)\/(\d+\.\d+\.\d+)\/?$/ );




/**************\
|*   SERVER   *|
\**************/
//Open logfile
try {
    log.init( logfile );
} catch( err ) {
    console.error( "An error occurred opening \"" + logfile + "\". Details: " + err );
    process.exit( 1 );
}

//Load config
try {
    var config = JSON.parse( fs.readFileSync( configfile ) );
    mysql_params.user     = config.user;
    mysql_params.password = config.password;
    mysql_params.database = config.database;
} catch( err ) {
    console.error( "An error occurred reading \"" + configfile + "\". Details: " + err );
    process.exit( 1 );
}

//Create the server and listen on the given port
http.createServer( function( req, res ) {
    log_request( req );
    var conn = new Connection( req, res );
    conn.startTimeout();

    //Ensure correct method being used
    if( req.method != "GET" )
        return conn.error( "Incorrect method.", 405 );

    //Connect to the database.
    conn.sql = mysql2.createConnection( mysql_params );

    //Parse URL for meaningful information
    conn.url = url.parse( req.url );

    //Try to find some route that can satisfy the request, or fail with a 404
    if( !try_routes( conn ) )
        return conn.error( "Route not found.", 404 );
} ).listen( port );

log.out( "Server started on port " + port + "." );
console.log( "Press CTRL+C to exit." );