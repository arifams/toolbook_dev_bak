﻿/// <binding ProjectOpened='nggettext_compile:all' />
'use strict';

module.exports = function (grunt) {
    grunt.initConfig({
        nggettext_extract: {
            pot: {
                files: {
                    'po/template.pot': 'app/**/*.html'//['*.html']
                }
            },
        },

        nggettext_compile: {
            all: {
                files: {
                    'js/translations.js': ['po/*.po']
                }
            },
        },
    });

    grunt.loadNpmTasks('grunt-angular-gettext');

    grunt.registerTask('default', ['nggettext_extract', 'nggettext_compile']);
};
