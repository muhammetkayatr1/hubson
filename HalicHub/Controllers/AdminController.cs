using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Halic.Bussiness.Abstract;
using Halic.Entity;
using HalicHub.Extensions;
using HalicHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HalicHub.Controllers
{
    [Authorize]//BURAYA MUTLAKA YETKİLENDİRİLMİŞ GİRER DEMEK
    public class AdminController : Controller
    {
        private IArticleServices _articleServices;
        private ICategoryServices _categoryServices;
        private IAuthorServices _authorServices;
        private INewsServices _newsServices;
        private INCategoryServices _nCategoryServices;
        private IVideoServices _videoServices;
        private ISliderServices _sliderServices;
        private IActivitiesServices _activitiesServices;        

        public AdminController(IArticleServices articleServices, ICategoryServices categoryServices, IAuthorServices authorServices, INewsServices newsServices, INCategoryServices nCategoryServices, IVideoServices videoServices, ISliderServices sliderServices, IActivitiesServices activitiesServices)
        {
            _articleServices = articleServices;
            _categoryServices = categoryServices;
            _authorServices = authorServices;
            _newsServices = newsServices;
            _nCategoryServices = nCategoryServices;
            _videoServices = videoServices;
            _sliderServices = sliderServices;
            _activitiesServices = activitiesServices;
        }
        // Makale Operasyonları
        public IActionResult ArticleListAdmin()
        {   
            return View(new ArticleListViewModel {
              Articles=_articleServices.GetOrderAll()
            });
        }
        [HttpGet]
        public IActionResult ArticleCreateAdmin()
        {
            ViewBag.Categories = _categoryServices.GetAll();
            ViewBag.Authors = _authorServices.GetAll();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ArticleCreateAdmin(ArticleModel model, int[] categories, int[] authors, IFormFile file)
        {
                     
                var entity = new Article()
                {
                    Title = model.Title,
                    Content = model.Content,
                    Description = model.Description,
                    Date = model.Date,
                    img = model.img,
                    Url = model.Url,
                    IsApproved = model.IsApproved,
                };

                if (file != null)
                {
                    var extention = Path.GetExtension(file.FileName);
                    var randomName = string.Format($"{Guid.NewGuid()}{extention}");
                    entity.img = randomName;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", randomName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                _articleServices.ArticleCreate(entity, categories, authors);
                TempData.Put("message", new AlertMessage()
                {
                    Title = "Yeni Makale",
                    Message = $"{model.Title} Makale Başarıyla Eklendi",
                    AlertType = "success"
                });

                return RedirectToAction("ArticleListAdmin");
        }

        [HttpGet]
        public IActionResult ArticleEditAdmin(int? id)     
        {   
            var entity = _articleServices.GetByWithCategoriesId((int)id);

            var model = new ArticleModel()
            {
                ArticleId=entity.ArticleId,
                Title = entity.Title,
                Content = entity.Content,
                Description = entity.Description,
                Date = entity.Date,
                img = entity.img,
                Url = entity.Url,
                IsApproved = entity.IsApproved,
                Categories=entity.ArticleCategories.Select(i=>i.Categories).ToList(),
                Authors = entity.ArticleAuthors.Select(i=>i.Authors).ToList()
            };
            ViewBag.Categories = _categoryServices.GetAll();
            ViewBag.Authors = _authorServices.GetAll();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ArticleEditAdmin(ArticleModel model,int [] categoryIds,int[] authors,IFormFile file)
        {
            var entity = _articleServices.GetById(model.ArticleId);

            entity.Title = model.Title;
            entity.Content = model.Content;
            entity.Description = model.Description;
            entity.Date = model.Date;
            entity.Url = model.Url;
            entity.IsApproved = model.IsApproved;

            if (file != null)
            {
                var extention = Path.GetExtension(file.FileName);
                var randomName = string.Format($"{Guid.NewGuid()}{extention}");
                entity.img = randomName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", randomName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }

            _articleServices.Update(entity,categoryIds,authors);

            TempData.Put("message", new AlertMessage()
            {
                Title = "Makale Güncellendi",
                Message = $"{model.Title} Makale Başarıyla Güncellendi",
                AlertType = "warning"
            });
            return RedirectToAction("ArticleListAdmin");
        }

        public IActionResult ArticleDeleteAdmin(int ArticleId)
        {
            var entity = _articleServices.GetById(ArticleId);
            var tittle = entity.Title;
            _articleServices.Delete(entity);

            TempData.Put("message", new AlertMessage()
            {
                Title = "Makale Silindi",
                Message = $"{tittle} Makale Başarıyla Silindi",
                AlertType = "danger"
            });
            return RedirectToAction("ArticleListAdmin");
        }
        //-----------------------------------------------------------
        //MakaleKategori sayfası
        public IActionResult ArticleCategoryListAdmin()
        {
            return View(new CategoryListViewModel
            {
                Categories = _categoryServices.GetAll()
            });
        }
        [HttpGet]
        public IActionResult ArticleCategoryCreateAdmin()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ArticleCategoryCreateAdmin(CategoryModel model, IFormFile file)
        {
            var entity = new Category()
            {
                Name = model.Name,
                Url = model.Url,
            };
            if (file != null)
            {
                var extention = Path.GetExtension(file.FileName);
                var randomName = string.Format($"{Guid.NewGuid()}{extention}");
                entity.ImageUrl = randomName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", randomName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            _categoryServices.Create(entity);

            TempData.Put("message", new AlertMessage()
            {
                Title = "Yeni Kategori",
                Message = $"{entity.Name} Kategorisi Başarıyla Eklendi",
                AlertType = "success"
            });
            return RedirectToAction("ArticleCategoryListAdmin");
        }

        [HttpGet]
        public IActionResult ArticleCategoryEditAdmin(int id)
        {
            var entity = _categoryServices.GetById(id);
            var model = new CategoryModel()
            {
                CategoryId = entity.CategoryId,
                Name = entity.Name,
                Url = entity.Url,
                ImageUrl = entity.ImageUrl
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ArticleCategoryEditAdmin(CategoryModel model, IFormFile file)
        {
            var entity = _categoryServices.GetById(model.CategoryId);
            entity.CategoryId = model.CategoryId;
            entity.Name = model.Name;
            entity.Url = model.Url;

            if (file != null)
            {
                var extention = Path.GetExtension(file.FileName);
                var randomName = string.Format($"{Guid.NewGuid()}{extention}");
                entity.ImageUrl = randomName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", randomName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            _categoryServices.Update(entity);
            TempData.Put("message", new AlertMessage()
            {
                Title = "Kategori Gümcelleme",
                Message = $"{entity.Name} Kategorisi Başarıyla Güncellendi",
                AlertType = "warning"
            });
            return RedirectToAction("ArticleCategoryListAdmin");
        }
        [HttpPost]
        public IActionResult ArticleCategoryDeleteAdmin(int CategoryId)
        {
            var entity = _categoryServices.GetById(CategoryId);
            _categoryServices.Delete(entity);
            var name = entity.Name;
            TempData.Put("message", new AlertMessage()
            {
                Title = "Kategori Silme",
                Message = $"{name} Kategorisi Başarıyla Silindi",
                AlertType = "danger"
            });
            return RedirectToAction("ArticleCategoryListAdmin");
        }
        //----------------------------------------------------------------------------------------
        // Yazar Operasyonları  AUTHOR

        public IActionResult AuthorListAdmin()              
        {
            return View(new AuthorListViewModel
            {
                Authors = _authorServices.GetOrderAll() 
            });
        }
        [HttpGet]
        public IActionResult AuthorCreateAdmin()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AuthorCreateAdmin(AuthorModel model, IFormFile file)
        {
            var entity = new Author()
            {
                NameSurname = model.NameSurname,
                Description = model.Description,
                Url = model.Url
            };
            if (file != null)
            {
                var extention = Path.GetExtension(file.FileName);
                var randomName = string.Format($"{Guid.NewGuid()}{extention}");
                entity.Image = randomName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", randomName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            _authorServices.Create(entity);//burada file vermene gerek yok cshtmlde enctype multipleformdata yaptık bu file dosyasını servera taşı demek
            TempData.Put("message", new AlertMessage()
            {
                Title = "Yeni Yazar Ekleme",
                Message = $"{model.NameSurname} Başarıyla Eklendi",
                AlertType = "success"
            });
            return RedirectToAction("AuthorListAdmin");
        }

        [HttpGet]
        public IActionResult AuthorEditAdmin(int? id)   
        {
            var entity = _authorServices.GetById((int) id);

            var model = new AuthorModel()
            { 
                AuthorId=entity.AuthorId,
                NameSurname = entity.NameSurname,
                Description = entity.Description,
                Url=entity.Url,
                Image = entity.Image
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AuthorEditAdmin(AuthorModel model, IFormFile file)         
        {
            var entity = _authorServices.GetById(model.AuthorId);

            entity.AuthorId = model.AuthorId;
            entity.NameSurname = model.NameSurname;
            entity.Description = model.Description;
            entity.Url = model.Url;

            if (file != null)
            {
                var extention = Path.GetExtension(file.FileName);
                var randomName = string.Format($"{Guid.NewGuid()}{extention}");
                entity.Image = randomName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", randomName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            _authorServices.Update(entity);
            TempData.Put("message", new AlertMessage()
            {
                Title = "Yazar Güncelleme",
                Message = $"{model.NameSurname} Yazarı Başarıyla Güncellendi",
                AlertType = "warning"
            });
            return RedirectToAction("AuthorListAdmin");
        }

        public IActionResult AuthorDeleteAdmin(int AuthorId)   //authorId list.cshtmldeki alandada yazıyor ben buraya id diyodum ve silmiyodu sonra anladımki cshtmldeki name alanına ne yzarsam burayada onu yazmalıyım  
        {
            var entity = _authorServices.GetById(AuthorId);
            var name = entity.NameSurname;
            _authorServices.Delete(entity);
            TempData.Put("message", new AlertMessage()
            {
                Title = "Yazar Silme",
                Message = $"{name} Yazarı Başarıyla Silindi",
                AlertType = "danger"
            });
            return RedirectToAction("AuthorListAdmin");
        }
        //----------------- Haberler ------------------------
        public IActionResult NewsListAdmin()        
        {
            return View(new NewsListViewModel
            {
                News=_newsServices.GetOrderAll()    
            });
        }
        [HttpGet]
        public IActionResult NewsCreateAdmin()         
        {
            ViewBag.NCategories = _nCategoryServices.GetAll();                  
            ViewBag.Authors = _authorServices.GetAll();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> NewsCreateAdmin(ArticleModel model, int[] ncategories, int[] authors, IFormFile file)
        {
            var entity = new News() 
            {
                Title=model.Title,
                Content=model.Content,
                Description=model.Description,
                Date=model.Date,
                Url=model.Url,
                IsApproved=model.IsApproved
            };

            if (file != null)
            {
                var extention = Path.GetExtension(file.FileName);
                var randomName = string.Format($"{Guid.NewGuid()}{extention}");
                entity.img = randomName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", randomName);

                //ViewBag.NCategories = _nCategoryServices.GetAll();
                //ViewBag.Authors = _authorServices.GetAll();

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            _newsServices.NewsCreate(entity, ncategories, authors);
            TempData.Put("message", new AlertMessage()
            {
                Title = "Yeni Haber Ekleme",
                Message = $"{model.Title} Haberi Başarıyla Eklendi",
                AlertType = "success"
            });
            return RedirectToAction("NewsListAdmin");
        }

        [HttpGet]
        public IActionResult NewsEditAdmin(int id)
        {
            var entity = _newsServices.GetByWithCategoriesAndAuthorId(id);

            var model = new NewsModel()
            {
                NewsId=entity.NewsId,
                Title=entity.Title,
                Content=entity.Content,
                Description=entity.Description,
                Date=entity.Date,
                img=entity.img,
                Url=entity.Url,
                IsApproved=entity.IsApproved,
                nCategories= entity.newsHCategories.Select(i => i.NCategories).ToList(),
                Authors = entity.newsAuthors.Select(i => i.Author).ToList()
            };
            ViewBag.NCategories = _nCategoryServices.GetAll();
            ViewBag.Authors = _authorServices.GetAll();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> NewsEditAdmin(NewsModel model, int[] ncategories, int[] authors, IFormFile file)
        {
            var entity = _newsServices.GetByWithCategoriesAndAuthorId(model.NewsId);
            model.Title = entity.Title;
            model.Content = entity.Content;         
            model.Description = entity.Description;
            model.Date = entity.Date;
            model.Url = entity.Url; 
            model.IsApproved = entity.IsApproved;

            if (file != null)
            {
                var extention = Path.GetExtension(file.FileName);
                var randomName = string.Format($"{Guid.NewGuid()}{extention}");
                entity.img = randomName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", randomName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            _newsServices.Update(entity, ncategories, authors);
            TempData.Put("message", new AlertMessage()
            {
                Title = "Haber Güncelleme",
                Message = $"{model.Title} Haberi Başarıyla Güncellendi",
                AlertType = "warning"
            });
            return RedirectToAction("NewsListAdmin");
        }

        public IActionResult NewsDeleteAdmin(int NewsId)   //authorId list.cshtmldeki alandada yazıyor ben buraya id diyodum ve silmiyodu sonra anladımki cshtmldeki name alanına ne yzarsam burayada onu yazmalıyım  
        {
            var entity = _newsServices.GetById(NewsId);
            var name = entity.Title;
            _newsServices.Delete(entity);
            TempData.Put("message", new AlertMessage()
            {
                Title = "Haber Silme",
                Message = $"{name} Haberi Başarıyla Silindi",
                AlertType = "danger"
            });
            return RedirectToAction("NewsListAdmin");
        }
        //---------- HaberKategori 

        public IActionResult NewsCategoryListAdmin()    
        {
            return View(new NCategoryListViewModel
            {
                nCategories = _nCategoryServices.GetAll()
            });
        }

        [HttpGet]
        public IActionResult NewsCategoryCreateAdmin()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> NewsCategoryCreateAdmin(NCategoryModel model,IFormFile file)
        {
            var entity = new NCategory()
            {
                Name = model.Name,
                Url = model.Url,
            };
            if (file != null)
            {
                var extention = Path.GetExtension(file.FileName);
                var randomName = string.Format($"{Guid.NewGuid()}{extention}");
                entity.ImageUrl = randomName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", randomName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            _nCategoryServices.Create(entity);
            var name = entity.Name;
            TempData.Put("message", new AlertMessage()
            {
                Title = "Kategori Ekleme",
                Message = $"{name} Kategorisi Başarıyla Eklendi",
                AlertType = "success"
            });
            return RedirectToAction("NewsCategoryListAdmin");
        }
        [HttpGet]
        public IActionResult NewsCategoryEditAdmin(int id)
        {
            var entity = _nCategoryServices.GetById(id);
            var model = new NCategoryModel()
            {
                NCategoryId = entity.NCategoryId,
                Name = entity.Name,
                ImageUrl = entity.ImageUrl,
                Url = entity.Url
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> NewsCategoryEditAdmin(NCategoryModel model,IFormFile file) 
        {
            var entity = _nCategoryServices.GetById(model.NCategoryId);
            entity.Name = model.Name;
            entity.Url = model.Url;

            if (file != null)
            {
                var extention = Path.GetExtension(file.FileName);
                var randomName = string.Format($"{Guid.NewGuid()}{extention}");
                entity.ImageUrl = randomName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", randomName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            _nCategoryServices.Update(entity);
            var name = entity.Name;
            TempData.Put("message", new AlertMessage()
            {
                Title = "Kategori Güncelleme",
                Message = $"{name} Kategorisi Başarıyla Güncellendi",
                AlertType = "warning"
            });
            return RedirectToAction("NewsCategoryListAdmin");
        }
        [HttpPost]
        public IActionResult NewsCategoryDeleteAdmin(int NCategoryId)  
        {
            var entity = _nCategoryServices.GetById(NCategoryId);
            var name = entity.Name;
            _nCategoryServices.Delete(entity);
            TempData.Put("message", new AlertMessage()
            {
                Title = "Kategori Silme",
                Message = $"{name} Kategorisi Başarıyla Silindi",
                AlertType = "danger"
            });
            return RedirectToAction("NewsCategoryListAdmin");
        }
        //------------------ Video CRUD

        public IActionResult VideoListAdmin()
        {
            return View(new VideoListViewModel { Videos=_videoServices.GetAll()});
        }

        [HttpGet]
        public IActionResult VideoCreateAdmin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VideoCreateAdmin(VideoModel model)
        {
            var entity = new Video()        
            {
                Tittle=model.Tittle,
                Url=model.Url,
                Date=model.Date
            };
            _videoServices.Create(entity);
            var name = entity.Tittle;
            TempData.Put("message", new AlertMessage()
            {
                Title = "Video Ekleme",
                Message = $"{name} Video Başarıyla Eklendi",
                AlertType = "success"
            });
            return RedirectToAction("VideoListAdmin");
        }

        [HttpGet]
        public IActionResult VideoEditAdmin(int id)
        {
            var entity = _videoServices.GetById(id);
            var model = new VideoModel()
            {
                Id=entity.Id,
                Tittle=entity.Tittle,
                Url=entity.Url,
                Date=entity.Date
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult VideoEditAdmin(VideoModel model)
        {
            var entity = _videoServices.GetById(model.Id);
            entity.Tittle = model.Tittle;
            entity.Url = model.Url;
            entity.Date = model.Date;

            _videoServices.Update(entity);

            var name = entity.Tittle;
            TempData.Put("message", new AlertMessage()
            {
                Title = "Video Güncelleme",
                Message = $"{name} Video Başarıyla Güncellendi",
                AlertType = "warning"
            });
            return RedirectToAction("VideoListAdmin");
        }

        [HttpPost]
        public IActionResult VideoDeleteAdmin(int VideoDelete)
        {
            var entity = _videoServices.GetById(VideoDelete);
            _videoServices.Delete(entity);
            var name = entity.Tittle;
            TempData.Put("message", new AlertMessage()
            {
                Title = "Video Silme",
                Message = $"{name} Video Başarıyla Silindi",
                AlertType = "danger"
            });
            return RedirectToAction("VideoListAdmin");
        }
        // SLİDER CRUD-------------------------------------------------------
        public IActionResult SliderListAdmin()
        {
            return View(new SliderListViewModel { Sliders = _sliderServices.GetOrderAll() });
        }
        [HttpGet]
        public IActionResult SliderCreateAdmin()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SliderCreateAdmin(SliderModel model,IFormFile file)
        {
            var entity = new Slider()
            {
                 Tittle=model.Tittle,
                 Description=model.Description,
                 Url=model.Url
            };                          
            if (file != null)
            {
                var extention = Path.GetExtension(file.FileName);
                var randomName = string.Format($"{Guid.NewGuid()}{extention}");
                entity.Resim = randomName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", randomName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            _sliderServices.Create(entity);
            var name = entity.Tittle;
            TempData.Put("message", new AlertMessage()
            {
                Title = "Slider Ekleme",
                Message = $"{name} Slider Başarıyla Eklendi",
                AlertType = "success"
            });
            return RedirectToAction("SliderListAdmin");
        }
        [HttpGet]
        public IActionResult SliderEditAdmin(int id)
        {
            var entity = _sliderServices.GetById(id);
            var model = new SliderModel()
            {
                SliderId=entity.SliderId,
                Tittle=entity.Tittle,
                Description=entity.Description,
                Resim=entity.Resim,
                Url=entity.Url
            };
            return View(model);
        }
        [HttpPost]  
        public IActionResult SliderEditAdmin(SliderModel model)
        {
            var entity = _sliderServices.GetById(model.SliderId);
            entity.Tittle = model.Tittle;
            entity.Description = model.Description;
            entity.Resim = model.Resim;
            entity.Url = model.Url;
            _sliderServices.Update(entity);
            var name = entity.Tittle;
            TempData.Put("message", new AlertMessage()
            {
                Title = "Slider Güncelleme",
                Message = $"{name} Slider Başarıyla Güncellendi",
                AlertType = "warning"
            });
            return RedirectToAction("SliderListAdmin");
        }
        [HttpPost]
        public IActionResult SliderDeleteAdmin(int SliderId)
        {
            var entity = _sliderServices.GetById(SliderId);
            var name = entity.Tittle;
            _sliderServices.Delete(entity);
            TempData.Put("message", new AlertMessage()
            {
                Title = "Slider Silme",
                Message = $"{name} Slider Başarıyla Silindi",
                AlertType = "danger"
            });
            return RedirectToAction("SliderListAdmin");
        }
        //---------- Etkinlikler Activities
        [HttpGet]
        public IActionResult ActivitiesListAdmin()
        {
            return View(new ActivitiesListViewModel { activities=_activitiesServices.GetOrderAll()});
        }
        [HttpGet]
        public IActionResult ActivitiesCreateAdmin()    
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ActivitiesCreateAdmin(ActivitiesModel model,IFormFile file)
        {
            var entity = new Activities()
            {
                Tittle=model.Tittle,
                Description=model.Description,
                Content=model.Content,
                Date=model.Date,
                Url=model.Url
            };
            if (file != null)
            {
                var extention = Path.GetExtension(file.FileName);
                var randomName = string.Format($"{Guid.NewGuid()}{extention}");
                entity.img = randomName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", randomName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            _activitiesServices.Create(entity);
            var name = entity.Tittle;
            TempData.Put("message", new AlertMessage()
            {
                Title = "Etkinlik Ekleme",
                Message = $"{name} Etkinlik Başarıyla Eklendi",
                AlertType = "success"
            });
            return RedirectToAction("ActivitiesListAdmin");
        }
        [HttpGet]
        public IActionResult ActivitiesEditAdmin(int id)
        {
            var entity = _activitiesServices.GetById(id);
            var model = new ActivitiesModel()
            {
                ActivityId=entity.ActivityId,
                Tittle=entity.Tittle,
                Description=entity.Description,
                Content=entity.Content,
                Date=entity.Date,
                img=entity.img,
                Url=entity.Url
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult ActivitiesEditAdmin(ActivitiesModel model)
        {
            var entity = _activitiesServices.GetById(model.ActivityId);
            entity.Tittle = model.Tittle;
            entity.Description = model.Description;
            entity.Content = model.Content;
            entity.Date = model.Date;
            entity.img = model.img;
            entity.Url = model.Url;
            _activitiesServices.Update(entity);
            var name = entity.Tittle;
            TempData.Put("message", new AlertMessage()
            {
                Title = "Etkinlik Güncelleme",
                Message = $"{name} Etkinlik Başarıyla Güncelleme",
                AlertType = "warning"
            });
            return RedirectToAction("ActivitiesListAdmin");
        }
        [HttpPost]
        public IActionResult ActivitiesDeleteAdmin(int ActivityId)
        {
            var entity = _activitiesServices.GetById(ActivityId);
            _activitiesServices.Delete(entity);
            var name = entity.Tittle;
            TempData.Put("message", new AlertMessage()
            {
                Title = "Etkinlik Silme",
                Message = $"{name} Etkinlik Başarıyla Silindi",
                AlertType = "danger"
            });
            return RedirectToAction("ActivitiesListAdmin");
        }
        [HttpGet]
        public IActionResult AdminPanel()
        {
            return View();
        }
    }
}
