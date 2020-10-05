using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HalicHub.Models;
using Halic.Data.Abstract;
using Halic.Bussiness.Abstract;
using Halic.Entity;

namespace HalicHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IArticleServices _articleServices;
        private ICategoryServices _categoryServices;
        private IAuthorServices _authorServices;
        private ISliderServices _sliderServices;
        private INewsServices _newsServices;
        private INCategoryServices _nCategoryServices;
        private IActivitiesServices _activitiesServices;
        private IVideoServices _videoServices;
                


        public HomeController(ILogger<HomeController> logger,IArticleServices articleServices,ICategoryServices categoryServices,IAuthorServices authorServices, ISliderServices sliderServices, INewsServices newsServices, INCategoryServices nCategoryServices, IActivitiesServices activitiesServices, IVideoServices videoServices)
        {
            _logger = logger;
            _articleServices = articleServices;
            _categoryServices = categoryServices;
            _authorServices = authorServices;
            _sliderServices = sliderServices;
            _newsServices = newsServices;
            _nCategoryServices = nCategoryServices;
            _activitiesServices = activitiesServices;
            _videoServices = videoServices;
        }

        public IActionResult Index()
        {
            SliderListViewModel sliderListViewModel = new SliderListViewModel()
            {
                Sliders = _sliderServices.GetAll()
            };
            return View(sliderListViewModel);
        }
        public IActionResult ArticleList(string category,int page=1)  
        {
            const int pageSize = 15;
            ArticleListViewModel ArticleListView = new ArticleListViewModel()
            {
                pageInfo=new PageInfo()
                {
                    TotalItems=_articleServices.GetCountByCategory(category),
                    CurrentPage=page,
                    ItemsPerPage=pageSize,
                    CurrentCategory=category
                },
                Articles = _articleServices.GetArticleByCategory(category,page,pageSize)
            };
            return View(ArticleListView);
        }
        public IActionResult ArticleDetails(string url)
        {
            if(url == null)
            {
                return NotFound();
            }
            Article article = _articleServices.GetArticleDetails(url);
           
            return View(new ArticleDetailsModel
            {
                Articles = article,
                Authors = article.ArticleAuthors
               .Select(i => i.Authors)
               .ToList(),
               Categories=article.ArticleCategories
               .Select(i=>i.Categories)
               .ToList()
            });
        }
            
        public IActionResult AuthorList()   
        {
            AuthorListViewModel authorListViewModel = new AuthorListViewModel()
            {
                Authors =_authorServices.GetAll()
            };
            return View(authorListViewModel);
        }

        public IActionResult AuthorDetails(string author)
        {
            if (author == null)
            {
                return NotFound();
            }
            Author Author = _authorServices.GetAuthorDetails(author);


            var model = new AuthorDetailModel
            {
                Authors = Author,
                Articles = Author.ArticleAuthors
               .Select(i => i.Articles).ToList(),
                News = Author.newsAuthors
               .Select(i => i.News).ToList(),
            };
            return View(model);
        }

        public IActionResult search(string q)
        {
            var aricleListViewModel = new ArticleListViewModel()
            {
                Articles = _articleServices.GetSearchResult(q)
            };
            return View(aricleListViewModel);
        }
        public IActionResult NewsSearch(string q)
        {
            var newsListViewModel = new NewsListViewModel()
            {
                News = _newsServices.GetSearchResult(q)
            };
            return View(newsListViewModel);
        }
        public IActionResult AuthorSearch(string q)
        {
            var authorListViewModel = new AuthorListViewModel()
            {
                Authors = _authorServices.GetSearchResult(q)
            };
            return View(authorListViewModel);
        }
        public IActionResult NewsList(string categories, int page = 1)
        {
            const int pageSize = 2;
            NewsListViewModel newsListViewModel = new NewsListViewModel()
            {
                pageInfo = new PageInfoNews()
                {
                    TotalItems = _newsServices.GetCountByCategory(categories),
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    CurrentCategory = categories
                },
                News = _newsServices.GetNewsByCategory(categories, page, pageSize)
            };
            return View(newsListViewModel);
        }

        public IActionResult NewsDetails(string url)  
        {
            News news = _newsServices.GetNewsDetails(url);
            return View(new NewsDetailModel
            {
                News=news,
                Categories=news.newsHCategories.Select(i=>i.NCategories).ToList(),
                Authors=news.newsAuthors.Select(i=>i.Author).ToList()
            });
        }

        public IActionResult ActivitiesDetails(string detail)        
        {
            Activities activity = _activitiesServices.GetActivitiesDetails(detail);

            return View(new ActivitiesDetailsModel
            {
                activities = activity
            }); 
        }

        public IActionResult Video()    
        {
            VideoListViewModel videoListViewModel = new VideoListViewModel()
            {
                Videos = _videoServices.GetAll()
            };

            return View(videoListViewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
