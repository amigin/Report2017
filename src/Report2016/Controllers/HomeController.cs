using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Report2016.Models;
using Reports2016.Domains;

namespace Report2016.Controllers
{
	
    public class HomeController : Controller
    {
	    readonly IVoteTokensRepository _voteTokensRepository;
	    readonly IVotesRepository _votesRepository;

        public HomeController(IVoteTokensRepository voteTokensRepository, IVotesRepository votesRepository)
        {
            _voteTokensRepository = voteTokensRepository;
            _votesRepository = votesRepository;
        }


        private string urlToRedirect = "https://www.lykke.com/Annual_Report_2016.pdf";

		[HttpGet("/")]
        public IActionResult Index()
        {
			//return View("Result");

			var usereEmail = this.GetUserEmail();

            if (string.IsNullOrEmpty(usereEmail))
                return RedirectToAction("Signin");

            return RedirectToAction("Vote");
	}

		[Authorize]
		public IActionResult DoSignIn()
		{
			//return Redirect(urlToRedirect);
			return RedirectToAction("Index");
		}


		[Authorize]
		public IActionResult Auth()
		{
			//return Redirect(urlToRedirect);
			return RedirectToAction("Index");
		}

        [HttpGet("/Result")]
		public IActionResult Result()
		{
			return Redirect(urlToRedirect);
			// return View();
		}

		[HttpGet("/Success")]
		public IActionResult Success()
		{
			//return Redirect(urlToRedirect);
			return View();
		}

		[HttpGet("/Signin")]
		public IActionResult Signin()
		{
			//return Redirect(urlToRedirect);
			return View();
		}


		[HttpGet("/Vote")]
        [Authorize]
        public async Task<IActionResult> Vote()
		{
			//return Redirect(urlToRedirect);
            
            var email = this.GetUserEmail();
			var vote = await _votesRepository.GetAsync(email);

            if (vote != null)
				return RedirectToAction("Success");


            var user = this.GetUser();

            var viewModel = new VoteViewModel{
                User = user.FirstName+" "+user.LastName
            };
            return View(viewModel);
		}


		[HttpPost("/MyVote")]
		public async Task<IActionResult> MyVote(MyVoteContract model)
		{

			//return Redirect(urlToRedirect);
            

            if (model.NotVoted())
				return RedirectToAction("Vote");
            
            if (string.IsNullOrEmpty(model.Token))
            {
                var user = this.GetUser();

                if (user == null)
                    return RedirectToAction("Signin");

                model.Email = user.Email;
                model.UserId = user.UserId;

            }
            else
            {
                var token = await _voteTokensRepository.FindTokenAsync(model.Token);

                if (token == null)
					return RedirectToAction("Signin");

                model.Email = token.Email;
            }

            IVote vote = await _votesRepository.GetAsync(model.Email);

            if (vote != null)
                return RedirectToAction("Success");



            await _votesRepository.VoteAsync(model);

            return RedirectToAction("Success");



		}


        [HttpGet("/Vote/{id}")]
		public async Task<IActionResult> Vote([FromRoute]string id)
		{
			//return Redirect(urlToRedirect);
            

            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");

            var token = await _voteTokensRepository.FindTokenAsync(id);

            if (token == null)
                return RedirectToAction("Index");



            var vote = await _votesRepository.GetAsync(token.Email);

            if (vote != null)
				return RedirectToAction("Success");               

            var viewModel = new VoteViewModel
            {
                Token = id,
                User = token.FullName
            };

			return View("Vote", viewModel);

		}

    }
	
}
