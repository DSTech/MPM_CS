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



/**************\
|* CONSTANTS  *|
\**************/
var headers             = { "Content-Type": "application/json", "Cache-Control": "max-age=0" };
var error_headers       = { "Content-Type": "text/plain",       "Cache-Control": "max-age=0" };
var ipv6_is_ipv4_prefix = "::ffff:";




/**************\
|*  GLOBALS   *|
\**************/
var routes = [];
var logfile = fs.createWriteStream( "log.txt", { "flags": "a" } );
var user;
var password;



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
 * Writes the given string both the console and logfile
 * @param {String} str - The string to log to the console and logfile
 */
function log( str ) {
    logfile.write( str + "\n" );
    console.log( str )
}

/**
 * Logs details of a request made to the server.
 * @param {http.IncomingMessage} req - The request
 */
function log_request( req ) {
    //Log detailed error message including user's IP, request method and url, and code delivered to the user
    var useragent = req.headers["user-agent"];
    log(
        "[" + strftime( "%d-%b-%yT%H:%M:%S" ) + "] " +
        trim_ip( req.socket.remoteAddress )           +
        " "                                           +
        ( useragent ? "(" + useragent + ") " :  " " ) +
        req.method                                    +
        " "                                           +
        req.url
    );
}

/**
 * Call this when a server error occurs.
 * It will log the error to the console and generate the appropriate status code & error document to return to the user.
 * @param {http.ServerResponse} res - The response
 * @param {Number} code - The HTTP response code to return to the user
 * @param {String} message - An error message to display to the user.
 */
function server_error( res, code, message ) {
    //Log the error message
    console.error( "ERROR: " + message );

    //Send error message back to client
    res.writeHeader( code, error_headers );
    res.end( "HTTP/1.1 " + String( code ) + ": " + message );
}

/**
 * Add a route the server can handle.
 * @param {Function} callback - Callback function.
 *     Callbacks take as arguments:
 *         {String} path - the full path that was matched.
 *         {http.ServerResponse} res - The HTTP response object for the current connection being handled.
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
 * Given a path, try to find a route that can satisfy it.
 * @param {String} path - The path to match to a route (e.g. "/forge/1.2.3")
 * @returns {Boolean} true if the url matched a route, false otherwise
 */
function try_routes( info ) {
    //Note: using .some() instead of .forEach() here
    //because it needs to terminate once a route has been found.
    //In the function below, return true acts as a "break" statement,
    //whereas return false acts as "continue"
    return routes.some( function( route ) {
        var result = route.regex.exec( info.url.pathname );
        if( result == null )
            return false;

        route.callback.apply( null, [info].concat( result.slice( 1 ) ) );
        return true;
    } );
}




/**************\
|*   ROUTES   *|
\**************/

//List of all packages.
//Examples: "" or "/"
add_route( function( info ) {
    info.res.writeHeader( 200, headers );
    /*
    "SELECT "                                    +
        "m.name AS minecraft, "                  +
        "p.id AS package_id, "                   +
        "p.name AS package, "                    +
        "pv1.ver AS latest, "                    +
        "pv2.ver AS recommended "                +
    "FROM       MinecraftPackageVersion AS mpv " +
    "INNER JOIN Minecraft               AS m "   +
        "ON mpv.minecraft_id = m.id "            +
    "INNER JOIN Package                 AS p "   +
        "ON mpv.package_id = p.id "              +
    "INNER JOIN PackageVersion          AS pv1 " +
        "ON mpv.newest_id = pv1.id "             +
    "LEFT JOIN  PackageVersion          AS pv2 " +
        "ON mpv.recommended_id = pv2.id;"
    */

    /*
    //Build the JSON we'll be returning
    var packagesByID = {};
    var packages = [];
    var row;
    var package;
    for( var i = 0; i < rows.length; ++i ) {
        row = rows[i];
        package = packagesByID[row.package_id];
        if( package === undefined ) {
            package = {
                "id":          row.package_id,
                "name":        row.package,
                "latest":      {},
                "recommended": {}
            };
            packagesByID[row.package_id] = package;
            packages.push( package );
        }
        package.latest[row.minecraft] = row.latest;
        package.recommended[row.minecraft] = ( row.recommended === null ) ? row.latest : row.recommended;
    }
    */
    info.sql.query(
        "SELECT name, last_update as lastUpdated FROM Package",
        function( err, rows, fields ) {
            if( err ) {
                console.error( String( err ) );
                return server_error( info.res, 500, "MySQL error!" );
            }

            info.res.writeHeader( 200, headers );
            info.res.end( JSON.stringify( rows ) );
        }
    );
}, /^\/packages\/?$/ );

//Describe a specific package.
//Examples: "/forge" or "/forge/"
add_route( function( info, package ) {
    info.sql.query(
        "SELECT " +
            "m.name AS minecraft, " +
            "p.id AS package_id, " +
            "p.name AS package, ",
        function( err, rows, fields ) {
            if( err ) {
                console.error( String( err ) );
                return server_error( info.res, 500, "MySQL error!" );
            }

            //Build the JSON we'll be returning
            info.res.writeHeader( 200, headers );
            info.res.end( "{ \"route\": \"package\", \"package\": \"" + package + "\" }" );
        }
    );
}, /^\/packages\/([^/]+)\/?$/ );

//Describe a specific version of a package.
//Examples: "/forge/1.2.3" or "/forge/1.2.3/"
add_route( function( info, package, version ) {
    info.res.writeHeader( 200, headers );
    info.res.end( "{ \"route\": \"package_version\", \"package\": \"" + package + "\", \"version\": \"" + version + "\" }" );
}, /^\/packages\/([^/]+)\/(\d+\.\d+\.\d+)\/?$/ );




/**************\
|*   SERVER   *|
\**************/
//Load config
try {
    var config = JSON.parse( fs.readFileSync( "config.json" ) );
    user     = config.user;
    password = config.password;
    database = config.database;
} catch( err ) {
    console.error( err );
    process.exit( 1 );
}

//Create the server and listen on the given port
http.createServer( function( req, res ) {
    log_request( req );

    //Ensure correct method being used
    if( req.method != "GET" )
        return server_error( res, 405, "Incorrect method." );

    //Connect to the database.
    var sql = mysql2.createConnection({
        "user":     user,
        "password": password,
        "database": database
    });

    try {
        //Parse URL for meaningful information
        var url_bits = url.parse( req.url );
        var info = {
            "req": req,
            "res": res,
            "url": url_bits,
            "sql": sql
        };

        //Try to find some route that can satisfy the request, or fail with a 404
        if( !try_routes( info ) )
            return server_error( res, 404, "Route not found." );

    //Ensure database connection is closed
    } finally {
        sql.end();
    }
} ).listen( port );
