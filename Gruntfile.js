module.exports = function (grunt) {
    require('jit-grunt')(grunt);

    grunt.initConfig({
        less: {
            development: {
                options: {
                    compress: true,
                    yuicompress: true,
                    optimization: 2
                },
                files: {
                    "McsaMeetsMailer/wwwroot/css/fullSchedulePreview.css": "McsaMeetsMailer/wwwroot/css/fullSchedulePreview.less" // destination file and source file
                }
            }
        },
        watch: {
            styles: {
                files: ['McsaMeetsMailer/wwwroot/css/**/*.less'], // which files to watch
                tasks: ['less'],
                options: {
                    nospawn: true
                }
            }
        }
    });

    grunt.registerTask('default', ['less', 'watch']);
};