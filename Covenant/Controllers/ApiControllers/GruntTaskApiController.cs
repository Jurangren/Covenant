// Author: Ryan Cobb (@cobbr_io)
// Project: Covenant (https://github.com/cobbr/Covenant)
// License: GNU GPLv3

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq; // 添加此行
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Covenant.Core;
using Covenant.Models.Grunts;

namespace Covenant.Controllers
{
    [ApiController, Route("api/grunttasks"), Authorize(Policy = "RequireJwtBearer")]
    public class GruntTaskApiController : Controller
    {
        private readonly ICovenantService _service;

        public GruntTaskApiController(ICovenantService service)
        {
            _service = service;
        }

        // GET: api/grunttasks
        // <summary>
        // Get Tasks
        // </summary>
        [HttpGet(Name = "GetGruntTasks")]
        public async Task<ActionResult<IEnumerable<GruntTask>>> GetGruntTasks()
        {
            return Ok(await _service.GetGruntTasks());
        }

        // GET: api/grunttasks/{id}
        // <summary>
        // Get a Task by Id
        // </summary>
        [HttpGet("{id:int}", Name = "GetGruntTask")]
        public async Task<ActionResult<GruntTask>> GetGruntTask(int id)
        {
            try
            {
                return await _service.GetGruntTask(id);
            }
            catch (ControllerNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (ControllerBadRequestException e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST api/grunttasks
        // <summary>
        // Create a Task
        // </summary>
        [HttpPost(Name = "CreateGruntTask")]
        [ProducesResponseType(typeof(GruntTask), 201)]
        public async Task<ActionResult<GruntTask>> CreateGruntTask([FromBody] GruntTask task)
        {
            GruntTask savedTask = await _service.CreateGruntTask(task);
            return CreatedAtRoute(nameof(GetGruntTask), new { id = savedTask.Id }, savedTask);
        }

        // PUT api/grunttasks
        // <summary>
        // Edit a Task
        // </summary>
        [HttpPut(Name = "EditGruntTask")]
        public async Task<ActionResult<GruntTask>> EditGruntTask([FromBody] Newtonsoft.Json.Linq.JObject jsonBody)
        {
            // 将原始 JSON Body 反序列化为 GruntTask 对象
            GruntTask task = jsonBody.ToObject<GruntTask>();

            // 处理 ReferenceSourceLibraries
            if (jsonBody["ReferenceSourceLibraries"] != null && jsonBody["ReferenceSourceLibraries"].Type == Newtonsoft.Json.Linq.JTokenType.Array)
            {
                List<int> referenceSourceLibraryIds = new List<int>();
                foreach (var item in jsonBody["ReferenceSourceLibraries"])
                {
                    if (item["Id"] != null && item["Id"].Type == Newtonsoft.Json.Linq.JTokenType.Integer)
                    {
                        referenceSourceLibraryIds.Add(item["Id"].ToObject<int>());
                    }
                }
                // 调用服务层方法获取完整的 ReferenceSourceLibrary 对象
                task.ReferenceSourceLibraries = (await _service.GetReferenceSourceLibrariesByIds(referenceSourceLibraryIds)).ToList();
            }
            try
            {
                return await _service.EditGruntTask(task);
            }
            catch (ControllerNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (ControllerBadRequestException e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE api/grunttasks/{id}
        // <summary>
        // Delete a Task
        // </summary>
        [HttpDelete("{id}", Name = "DeleteGruntTask")]
        [ProducesResponseType(204)]
        public async Task<ActionResult> DeleteGruntTask(int id)
        {
            try
            {
                await _service.DeleteGruntTask(id);
            }
            catch (ControllerNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (ControllerBadRequestException e)
            {
                return BadRequest(e.Message);
            }
            return new NoContentResult();
        }
    }
}
