/// <binding Clean='clean' />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify");

var paths = {
    webroot: "./wwwroot/",
    node_modules: "./node_modules/"
};

var dependencies = {
    "jquery": {
        "dist/**": ""
    },
    "jquery-validation": {
        "dist/*": ""
    },
    "jquery-validation-unobtrusive": {
        "dist/**": ""
    },
    "bootstrap": {
        "dist/**/*": ""
    },
    "@fortawesome": {
        "fontawesome-free/css/**": "css",
        "fontawesome-free/webfonts/**": "webfonts"
    },
    "popper.js": {
        "dist/**": ""
    },
    "toastr": {
        "nuget/content/**": ""
    },
    "jquery-ajax-unobtrusive": {
        "jquery.unobtrusive-ajax.js": "",
        "jquery.unobtrusive-ajax.min.js": ""
    },
    "chart.js": {
        "dist/chart.js": "",
        "dist/chart.min.js": ""
    }
};

gulp.task('scripts',
    function(done) {
        var streams = [];
        // Scans the dependencies variable for all needed assest and copys them from node_modules to the lib
        for (var property in dependencies) {
            if (dependencies.hasOwnProperty(property)) {
                var safePropertyName = property.replace('@', '');
                if (dependencies.hasOwnProperty(property)) {
                    console.log(`Preparing scripts for ${property}.`);
                    var dependency = dependencies[property];
                    for (var item in dependency) {
                        if (dependency.hasOwnProperty(item)) {
                            streams.push(gulp.src(`${paths.node_modules}/${property}/${item}`)
                                .pipe(gulp.dest(`${paths.webroot}/lib/${safePropertyName}/${dependency[item]}`)));
                        }
                    }
                }
            }
        }
        done();
    });

paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/site.min.js";
paths.concatCssDest = paths.webroot + "css/site.min.css";

gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task("clean", gulp.series('clean:js', 'clean:css', function(done) {
    done();
}));

gulp.task("min:js", function () {
    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

gulp.task("min", gulp.series('min:js', 'min:css', function(done) {
    done();
}));