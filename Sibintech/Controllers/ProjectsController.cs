using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DBSettings;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sibintech.ShowModel;

namespace Sibintech.Controllers
{
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        [Route("[controller]/data")]
        [HttpGet]
        public ActionResult Data(){
            using ApplicationContext db = new ApplicationContext();
            var dbProjects = db.ProjTree.Where(p => p.Code.IndexOf("proj") > -1).ToList();
            List<TreeListModel> projects = new List<TreeListModel>();
            foreach (var pr in dbProjects)
            {
                projects.Add(new TreeListModel()
                {
                    Id = pr.Id,
                    Code = pr.Code,
                    Name = pr.Name,
                    Childrens = pr.Childrens,
                    CreatedDate = pr.CreatedDate,
                    RedactedDate = pr.RedactedDate,
                    ParentId = null,
                    HasChildren = pr.Childrens != null
                } );
            }
            return Ok(projects);
        }

        [Route("[controller]/childrens")]
        [HttpGet]
        public ActionResult Childrens(int parentId)
        {
            using ApplicationContext db = new ApplicationContext();
            var allObjects =  db.ProjTree.ToList();
            
            var selParent =  db.ProjTree.Where(pT => pT.Id == parentId);

            var childrensList = selParent.FirstOrDefault().Childrens.Split(',').Select(int.Parse).ToArray();
            
            List<TreeListModel> objects = new List<TreeListModel>();
            foreach (var aOb in allObjects)
            foreach (var chL in childrensList)
                    if (aOb.Id == chL)
                        objects.Add(new TreeListModel()
                        {
                            Id = aOb.Id,
                            Code = aOb.Code,
                            Name = aOb.Name,
                            Childrens = aOb.Childrens,
                            CreatedDate = aOb.CreatedDate,
                            RedactedDate = aOb.RedactedDate,
                            ParentId = parentId,
                            HasChildren = aOb.Childrens != null
                        });
            return Ok(objects);
        }
        
        [Route("[controller]/update")]
        [HttpPost]
        public async Task<ActionResult> Update(TreeListModel changedObj)
        {
            await using ApplicationContext db = new ApplicationContext();
            var allObjects =  db.ProjTree.ToList();
            var editedObj = allObjects
                .FirstOrDefault(aO => aO.Id == changedObj.Id);
            if (editedObj != null)
            {
                editedObj.Name = changedObj.Name;
                editedObj.Code = changedObj.Code;
                editedObj.RedactedDate = DateTime.Now;
                await db.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }
        
        [Route("[controller]/create")]
        [HttpPost]
        public async Task<ActionResult> Create(TreeListModel newObj)
        {
            await using ApplicationContext db = new ApplicationContext();

            var projObj = new ProjTreeModel(){
                Code = newObj.Code,
                Name = newObj.Name,
                CreatedDate = DateTime.Now,
                RedactedDate = DateTime.Now
            };
            try
            {
                await db.ProjTree.AddAsync(projObj);
                await db.SaveChangesAsync();
                var createdObj = db.ProjTree.ToList().FirstOrDefault(aO => aO.Name == newObj.Name);

                if (newObj.ParentName != null)
                {
                    var allObjects = db.ProjTree.ToList();
                    var parObj = allObjects
                        .FirstOrDefault(aO => aO.Code == newObj.ParentName);
                    if (parObj != null)
                    {
                        parObj.Childrens = parObj.Childrens + createdObj.Id;
                        await db.SaveChangesAsync();
                        
                        return Ok(new TreeListModel(){
                            Code = createdObj.Code,
                            Name = createdObj.Name,
                            CreatedDate = createdObj.CreatedDate,
                            RedactedDate = createdObj.RedactedDate,
                            ParentId = parObj.Id
                        });
                    }
                    return BadRequest();
                }
                return Ok(createdObj);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex);
            }
        }
        
        [Route("[controller]/delete")]
        [HttpPost]
        public async Task<ActionResult> Delete(TreeListModel delObj)
        {
            Console.WriteLine(delObj.Name);
            await using ApplicationContext db = new ApplicationContext();

            var delItem = new ProjTreeModel(){
                Id = delObj.Id
            };
            try
            {
                db.ProjTree.Attach(delItem);
                db.ProjTree.Remove(delItem);
                await db.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex);
            }
        }
    }
}