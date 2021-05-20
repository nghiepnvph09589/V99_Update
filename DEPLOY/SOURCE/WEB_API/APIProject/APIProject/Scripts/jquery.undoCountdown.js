(function ($) {

    $.fn.undoCountdown = function (options) {

        var defaults = {
            seconds: 5,
            term: 'Tiếp tục',
            showCountdown: true,
            onComplete: function () {
                return true;
            }
        };

        var settings = $.extend({}, defaults, options);

        return this.each(function () {

            var $button = $(this);
            var originalContent = $button.html();
            var seconds = settings.seconds;
            var term = settings.term;
            var showCountdown = settings.showCountdown;
            var interval;

            $button.click(function () {
                if (interval) {
                    reset();
                } else {
                    update();
                    start();
                }
            });

            function start() {
                if (!interval) {
                    interval = setInterval(update, 1000);
                }
            }

            function reset() {
                if (interval) {
                    $button.html(originalContent);
                    clearInterval(interval);
                    interval = null;
                    seconds = settings.seconds;
                }
            }

            function update() {

                if (showCountdown) {
                    $button.html("Quay lại app sau " + " (" + seconds + ")" + " giây");
                    $('#btnContinue').prop('disabled', true);
                } else {
                    $button.html(term);
                }

                if (seconds == 0) {
                    settings.onComplete.call();

                    if (interval) {
                        clearInterval(interval);
                    }
                } else {
                    seconds--;
                }
            }

        });

    }

}(jQuery));