var deviceAgent = navigator.userAgent.toLowerCase(),
  isMobile = deviceAgent.match(/(iphone|ipod|ipad)/),
  is_touch_device = ("ontouchstart" in window) || window.DocumentTouch && document instanceof DocumentTouch;

function initEventsOnResize() {
  $(window).resize(function() {
    var  wH = $(window).height();

    $('body').css({
      paddingBottom: $('footer').outerHeight()
    });

    $('.valign').css({
      minHeight: wH - ($('.r_header').outerHeight() + $('.footer').outerHeight()) - 50
    })
  }).trigger('resize');
}

function initEventsOnClick() {
  // Tel
  if (!isMobile) {
    $('body').on('click', 'a[href^="tel:"]', function() {
      $(this).attr('href',
        $(this).attr('href').replace(/^tel:/, 'callto:'));
    });
  }
}

function initEventsOnLoad() {
  $(window).on('load', function() {
    $('body').addClass('loaded');

    setTimeout(function() {
      $('.r_candle').each(function() {
        $(this).css({
          width: parseInt($(this).data('percent')) + '%'
        })
      });
    }, 100);
  })
}

function textareaCounter() {
  $("textarea#comment").keyup(function(){
    console.log('keyup', $(this).attr('maxlength'))
    var maxlength = $(this).attr('maxlength');
    $("._counter").text(maxlength - $(this).val().length);
  });
}

function resultsCounter() {
  var comma_separator_number_step = $.animateNumber.numberStepFactories.separator(',');

  $('.r_result__accent, .r_result__desc ._val').each(function() {
    $(this).animateNumber(
      {
        number: $(this).text().replace(',', ''),
        numberStep: comma_separator_number_step
      },
      1000
    );
  });
}

function toggleRadio() {
  $(".form_vote .radio").on('click', function() {
    $(this).siblings('.radio').find('input').removeAttr('checked');
    $(this).find('input').attr('checked', 'checked')
  });
}

function isSubmitEnabled() {
    var checkedCount = $('.form_vote .radio input:checked').length;
    if (checkedCount === 3 && $('.form_vote #comment').val().length) {
        $('.form_vote .btn').removeAttr('disabled');
    } else {
        $('.form_vote .btn').attr('disabled', 'disabled');
    }
}

function checkComment() {
    $(".form_vote #comment").on('keyup', function() {
      isSubmitEnabled();
    });
}

$(document).ready(function() {
  initEventsOnResize();
  initEventsOnClick();
  initEventsOnLoad();
  textareaCounter();
  resultsCounter();
  toggleRadio();
  checkComment();
});
