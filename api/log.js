var fs       = require( "fs" );

var logfile = null;

/**
 * Call this before calling any of the other functions in this module.
 * Opens the given file for appending and uses it for all future log output.
 * @param {String} filepath - The path of the file to use for log output.
 */
module.exports.init = function( filepath ) {
    logfile = fs.createWriteStream( filepath, { "flags": "a" } );
}

/**
 * Writes the given string both to stdout and the logfile
 * @param {String} str - The string to log to stdout and the logfile
 */
module.exports.out = function( str ) {
    logfile.write( str + "\n" );
    console.log( str );
}

/**
 * Writes the given string both to stderr and the logfile
 * @param {String} str - The string to log to stderr and logfile
 */
module.exports.err = function( str ) {
    logfile.write( str + "\n" );
    console.error( str );
}