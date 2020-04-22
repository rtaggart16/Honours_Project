/*
    Name: Ross Taggart
    ID: S1828840
*/

/// <binding BeforeBuild='default' />
module.exports = function (grunt) {
    'use strict';

    const sass = require('node-sass');

    // Project configuration.
    grunt.initConfig({
        pkg: grunt.file.readJSON('package-lock.json'),

        // Sass
        sass: {
            options: {
                sourceMap: true, // Create source map
                outputStyle: 'compressed', // Minify output,
                implementation: sass,
            },
            dist: {
                files: [
                    {
                        expand: true, // Recursive
                        cwd: "wwwroot/styles/scss", // The startup directory
                        src: ["**/*.scss"], // Source files
                        dest: "wwwroot/styles/css", // Destination
                        ext: ".css" // File extension
                    }
                ]
            }
        }
    });

    // Load the plugin
    grunt.loadNpmTasks('grunt-sass');

    // Default task(s).
    grunt.registerTask('default', ['sass']);
};