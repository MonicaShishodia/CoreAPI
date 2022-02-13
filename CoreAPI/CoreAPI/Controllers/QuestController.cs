using Entities.ViewModel;
using Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace CoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestController : ControllerBase
    {
        private readonly IImage _iImage;
        private readonly IQuest _iQuest;
        private readonly IConfiguration _config;
        public QuestController(IImage iImage, IQuest iQuest, IConfiguration configuration)
        {
            _iImage = iImage;
            _iQuest = iQuest;
            _config = configuration;
        }
        [HttpPost]
        [Route("/CreateQuest")]
        public string CreateQuest([FromForm] QuestViewModel questViewModel)
        {
            try
            {
                var identity = HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity;
                //dummy value
                identity.AddClaim(new System.Security.Claims.Claim("UserEmail", "test@test.com"));
                questViewModel.UploadedBy = identity?.FindFirst("UserEmail").Value;
                for (int i = 0; i < Request.Form.Files.Count; i++)
                {
                    var currentFile = Request.Form.Files[i];
                    if (currentFile != null && currentFile.Length > 0 && currentFile.ContentType.StartsWith("image/"))
                    {
                        string imageName = Guid.NewGuid().ToString();
                        string fileSystemPath = _config["MyConfig:NetworkPath"];
                        string fullPath = Path.Combine(fileSystemPath, imageName + Path.GetExtension(currentFile.FileName));
                        if (!System.IO.File.Exists(fullPath))
                        {
                            currentFile.CopyTo(new FileStream(fullPath, FileMode.Create));
                            questViewModel.Images.ElementAt(i).Path = fullPath;
                            questViewModel.Images.ElementAt(i).Name = imageName;
                            questViewModel.Images.ElementAt(i).SizeInBytes = currentFile.Length;
                        }
                        else throw new FileLoadException();
                    }
                    else throw new InvalidDataException();
                }
                if (questViewModel.Images.Any())
                {
                    return $"Quest created with ID : {_iQuest.AddQuest(questViewModel)}";
                }
                else { Response.StatusCode = StatusCodes.Status500InternalServerError; return "Please try again"; }
            }
            catch (Exception e)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return "Error!";
            }
        }
        [HttpGet]
        [Route("/GetQuestsByName/{QuestName}")]
        public IActionResult GetQuestsByName(string QuestName)
        {
            try
            {
                IEnumerable<QuestViewModel> q = _iQuest.GetQuestsByName(QuestName);
                if (q.Any())
                {
                    foreach (QuestViewModel obj in q)
                        AppendImageUrl(obj);
                    return Ok(q);
                }
                else return BadRequest();
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpGet]
        [Route("/GetQuestByID/{QuestID}")]
        public IActionResult GetQuestByID(int QuestID)
        {
            try
            {
                QuestViewModel q = AppendImageUrl(_iQuest.GetQuestByID(QuestID));
                if (q != null)
                    return Ok(q);
                else return BadRequest();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("/GetQuestsByPoint/{Latitude}/{Longtitude}")]
        public IActionResult GetQuestsByPoint(decimal Latitude, decimal Longtitude)
        {
            try
            {
                IEnumerable<QuestViewModel> q = _iQuest.GetQuestsByPoint(Latitude, Longtitude);
                if (q.Any())
                {
                    foreach (QuestViewModel obj in q)
                        AppendImageUrl(obj);
                    return Ok(q);
                }
                else return BadRequest();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("/GetQuestsByUser/{UserEmail}")]
        public IActionResult GetQuestsByUser(string UserEmail)
        {
            try
            {
                List<QuestViewModel> q = _iQuest.GetQuestsByUser(UserEmail).ToList();
                if (q.Count > 0)
                {
                    foreach (QuestViewModel obj in q)
                        AppendImageUrl(obj);
                    return Ok(q);
                }
                else return BadRequest();
            }
            catch
            {
                return BadRequest();
            }
        }

        //If API is behind a API gateway then all direction end point can be called as a result of single fetch from client /
        //so upstream may become /GetImagesByQuestID{QuestID}/ and Downstream becomes GetImagesByQuestIDAndDirection/{Quest}/{North/South/East/West}
        [HttpGet]
        [Route("/GetImagesByQuestIDAndDirection/{QuestID}/{Direction}")]
        public IActionResult GetImagesByQuestIDAndDirection(int QuestID, DirectionEnum Direction)
        {
            try
            {
                byte[] image = _iImage.GetImagesByQuestIDAndDirection(QuestID, Direction);
                if (image?.Length > 0)
                    return File(image, "image/jpeg");
                else return BadRequest();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("/GetImagesByQuestIDAndDirectionSize/{QuestID}/{Direction}/{ImageSize}")]
        public IActionResult GetImagesByQuestIDDirectionSize(int QuestID, DirectionEnum Direction, ImageSizeEnum ImageSize)
        {
            try
            {
                (int, int) size = (0, 0);
                size = Enum.GetName(typeof(ImageSizeEnum), ImageSize).ToLower() switch
                {
                    "thumbnail" => (128, 128),
                    "small" => (512, 512),
                    "large" => (2048, 2048),
                    _ => (0, 0)
                };

                byte[] image = _iImage.GetImagesByQuestIDDirectionSize(QuestID, Direction, size);
                if (image?.Length > 0)
                    return File(image, "image/jpeg");
                else return BadRequest();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }
        [HttpGet]
        [Route("/GetImagesByQuestIDDirectionCustom/{QuestID}/{Direction}/{Width}/{Height}")]
        public IActionResult GetImagesByQuestIDDirectionCustom(int QuestID, DirectionEnum Direction, int Width, int Height)
        {
            try
            {
                byte[] image = _iImage.GetImagesByQuestIDDirectionSize(QuestID, Direction, (Width, Height));
                if (image?.Length > 0)
                    return File(image, "image/jpeg");
                else return BadRequest();
            }
            catch
            {
                return BadRequest();
            }
        }
        private QuestViewModel AppendImageUrl(QuestViewModel q)
        {
            if (q != null)
            {
                foreach (ImageViewModel im in q?.Images)
                {
                    string[] imagePath = im.Path.Split(new string[] { _config["MyConfig:NetworkPath"] }, StringSplitOptions.None);
                    if (imagePath?.Length > 1)
                    {
                        im.ImageURL = $"{Request.Scheme}://{Request.Host.Value}/{_config["MyConfig:RequestPath"]}{imagePath[1].Replace("\\", "//")}";
                    }
                }
                return q;
            }
            else return null;
        }
    }
}
