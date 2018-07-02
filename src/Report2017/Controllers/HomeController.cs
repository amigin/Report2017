using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Report2017.Models;
using Reports2017.Domains;

namespace Report2017.Controllers
{
    public class HomeController : Controller
    {
        readonly IVoteTokensRepository _voteTokensRepository;
        readonly IVotesRepository _votesRepository;
        private const string UrlToRedirect = "https://www.lykke.com/Annual_Report_2016.pdf";

        public HomeController(IVoteTokensRepository voteTokensRepository, IVotesRepository votesRepository)
        {
            _voteTokensRepository = voteTokensRepository;
            _votesRepository = votesRepository;
        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            return View("VotingOver");
            /*var usereEmail = this.GetUserEmail();

            if (string.IsNullOrEmpty(usereEmail))
                return RedirectToAction("Signin");

            return RedirectToAction("Vote");*/
        }

        [Authorize]
        public IActionResult DoSignIn()
        {
            return View("VotingOver");
            //return Redirect(urlToRedirect);
            //return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult Auth()
        {
            return View("VotingOver");
            //return Redirect(urlToRedirect);
            //return RedirectToAction("Index");
        }

        [HttpGet("/Result")]
        public IActionResult Result()
        {
            return View("VotingOver");
            //return Redirect(UrlToRedirect);
            // return View();
        }

        [HttpGet("/Success")]
        public IActionResult Success()
        {
            return View("VotingOver");
            //return Redirect(urlToRedirect);
            //return View();
        }

        [HttpGet("/Signin")]
        public IActionResult Signin()
        {
            return View("VotingOver");
            //return Redirect(urlToRedirect);
            //return View();
        }

        [HttpGet("/Vote")]
        [Authorize]
        public async Task<IActionResult> Vote()
        {
            return View("VotingOver");
            /* 
            var user = this.GetUser();
            var vote = await _votesRepository.GetAsync(user.UserId);

            if (vote != null)
                return RedirectToAction("Success");

            var viewModel = new MyVoteContract
            {
                Name = user.FirstName + " " + user.LastName
            };

            return View(viewModel);*/
        }

        [HttpPost("/Vote")]
        public async Task<IActionResult> Vote(MyVoteContract model)
        {
            return View("VotingOver");
            /*
            if (model.Option1 == VoteOption.NotSure)
                ModelState.AddModelError(nameof(model.Option1), "Please answer this question");

            if (model.Option2 == VoteOption.NotSure)
                ModelState.AddModelError(nameof(model.Option2), "Please answer this question");

            if (model.Option3 == VoteOption.NotSure)
                ModelState.AddModelError(nameof(model.Option3), "Please answer this question");

            if (!string.IsNullOrEmpty(model.Comment) && model.Comment.Length > 500)
                ModelState.AddModelError(nameof(model.Comment), "Your question is too long. Max 500 chars allowed");

            if (!ModelState.IsValid)
                return View(model);

            if (model.NotVoted())
                return RedirectToAction("Vote");

            if (string.IsNullOrEmpty(model.Token))
            {
                var user = this.GetUser();

                if (user == null)
                    return RedirectToAction("Signin");

                model.UserId = user.UserId;
            }
            else
            {
                var token = await _voteTokensRepository.FindTokenAsync(model.Token);

                if (token == null)
                    return RedirectToAction("Signin");

            }

            var vote = await _votesRepository.GetAsync(model.UserId);

            if (vote != null)
                return RedirectToAction("Success");

            await _votesRepository.VoteAsync(model);

            return RedirectToAction("Success");
             */
        }

        [HttpGet("/Vote/{id}")]
        public async Task<IActionResult> Vote([FromRoute] string id)
        {
            return View("VotingOver");

            /* 
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");

            var token = await _voteTokensRepository.FindTokenAsync(id);

            if (token == null)
                return RedirectToAction("Index");

            var vote = await _votesRepository.GetAsync(token.Email);

            if (vote != null)
                return RedirectToAction("Success");

            var viewModel = new MyVoteContract
            {
                Token = id,
                Name = token.FullName
            };

            return View("Vote", viewModel);
            */
        }
    }
}
