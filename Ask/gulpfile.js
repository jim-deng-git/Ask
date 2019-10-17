var gulp = require("gulp"),
  sass = require("gulp-sass"),
  pug = require("gulp-pug"),
  //gulpUglify = require("gulp-uglify"),
  autoprefixer = require("gulp-autoprefixer"); //css前綴
  // sassGlob = require('gulp-sass-glob'),
  //filter = require("gulp-filter"),
  // gulpImagemin = require('gulp-imagemin'),
  //gulpPlumber = require("gulp-plumber"),
  //gulpNotify = require("gulp-notify"),
  //gulpRename = require("gulp-rename"),
  //del = require("del"),
  //babel = require("gulp-babel"); //es6轉es5

function errorLog(error) {
  console.error(error);
  //this.emit("end");
}

// sass
gulp.task("sass", function () {

  const stream = gulp
    .src("original/sass/**/*.sass")
    // .on('error', errorLog)
    .pipe(
      sass({
        outputStyle: "nested"
      })
    )
    .pipe(
      autoprefixer({
        overrideBrowserslist: ["last 5 versions", "Android >= 4.0"],
        cascade: true, //是否美化属性值 默认：true 像这样：
        remove: true //是否去掉不必要的前缀 默认：true
      })
    )
    .pipe(gulp.dest("css"));
  // .pipe(gulpNotify("CSS Finish"));

  return stream;
});

gulp.task("pug", function () {
  return gulp
    .src("original/views/*.pug")
    //.on("error", errorLog)
    //.pipe(gulpPlumber())
    .pipe(
      pug({
        pretty: true
      })
    )
    .pipe(gulp.dest("./"));
    //.pipe(gulpNotify("pug Finish"));
});

// watch
gulp.task("watch", function () {
  gulp.watch('original/sass/**/*.sass', gulp.series('sass'));
  gulp.watch("original/views/**/*.pug", gulp.series('pug'));
  gulp.watch("original/views/*.pug", gulp.series('pug'));
});
