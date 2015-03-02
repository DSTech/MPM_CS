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
add_route( function( conn, package ) {
    var results = {
        "name":    "",
        "authors": [],
        "builds":  []
    };
    var package_id;

    //Attempt to find the id and official name of a package with the given name.
    //Note: Prepared statements will automatically .escape() provided parameters.
    conn.sql.query(
        "SELECT id, name "    +
            "FROM Package "   +
            "WHERE name = ? " +
            "LIMIT 1",
        [ package ],
        function( err, rows, fields ) {
            if( err )
                return conn.mysql_error( err );
            if( rows.length == 0 )
                return conn.error( "Package not found.", 404 );

            //This package exists.
            //Grab the official name & id, then find builds and authors.
            results.name = rows[0].name;
            package_id   = rows[0].id;

            conn.sql.query(
                "SELECT ver, given, flags " +
                    "FROM Build "           +
                    "WHERE packageid = ?",
                [ package_id ],
                function( err, rows, fields ) {
                    if( err )
                        return con.mysql_error( err );

                    for( var i = 0; i < rows.length; ++i ) {
                        results.builds.push( {
                            "version":      rows[i].ver,
                            "givenVersion": rows[i].given,
                            "recommended":  Boolean( rows[i].flags & FLAG_RECOMMENDED ),
                            "interfaces":   [],
                            "dependencies": {
                                "packages":   [],
                                "interfaces": []
                            },
                            "conflicts":    [],
                            "hashes":       []
                        } );
                    }

                    conn.end( JSON.stringify( results ) );
                }
            );
        }
    );
}, /^\/packages\/([^/]+)\/?$/ );

//Describe a specific version of a package.
//Examples: "/packages/forge/1.2.3" or "/packages/forge/1.2.3/"
add_route( function( connection, package, version ) {
    connection.end( "{ \"route\": \"package_version\", \"package\": \"" + package + "\", \"version\": \"" + version + "\" }" );
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