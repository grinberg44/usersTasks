using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersTasks.Services;
using UsersTasks.Models;
using System;

namespace UsersTasks.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly TaskService _taskService;
        private readonly UserService _userService;

        public TasksController(TaskService taskService, UserService userService)
        {
            _taskService = taskService;
            _userService = userService;
        }

        // GET: api/Tasks
        [HttpGet]
        public ActionResult<List<Models.Task>> GetTasks()
        {
            try
            {
                var userIdClainm = User.Claims.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.InvariantCultureIgnoreCase));

                if (userIdClainm == null)
                {
                    throw new Exception("Token not valid");
                }

                List<Models.Task> tasks = _taskService.GetUserTasks(userIdClainm.Value);

                return Ok(tasks);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        // GET: api/Tasks/byemail
        [Authorize(Roles = "admin")]
        [HttpGet("byemail")]
        public ActionResult<List<Models.Task>> GetTasksByEmail([FromQuery]string email)
        {
            try
            {
                User user = _userService.GetUserByEmail(email);

                if (user == null)
                {
                    throw new Exception($"No user with given Email {email}");
                }

                List<Models.Task> tasks = _taskService.GetUserTasks(user.UserId.ToString());

                return Ok(tasks);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        // GET: api/Tasks/sendToemail
        [HttpGet("sendToemail")]
        public ActionResult<List<Models.Task>> SendTasksToEmail()
        {
            try
            {
                var userIdClainm = User.Claims.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.InvariantCultureIgnoreCase));

                if (userIdClainm == null)
                {
                    throw new Exception("Token not valid");
                }

                _taskService.SendTasksToUser(userIdClainm.Value);

                return Ok();
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        // POST: api/Tasks
        [HttpPost]
        public ActionResult<Models.Task> PostTask(Models.Task newTask)
        {
            try
            {
                var userIdClainm = User.Claims.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.InvariantCultureIgnoreCase));

                if (userIdClainm == null)
                {
                    throw new Exception("Token not valid");
                }

                _taskService.AddUserTask(newTask, userIdClainm.Value);

                return Ok(newTask);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        // POST: api/Tasks/share
        [HttpPost("share")]
        public IActionResult PostTask([FromBody] TasksShareData shareData)
        {
            try
            {
                var userIdClainm = User.Claims.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.InvariantCultureIgnoreCase));

                if (userIdClainm == null)
                {
                    throw new Exception("Token not valid");
                }

                bool isShared = _taskService.ShareTasksWithUser(shareData, userIdClainm.Value);

                if (!isShared)
                {
                    throw new Exception("Not shared");
                }

                return NoContent();
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        // PUT: api/Tasks/5
        [HttpPut("{taskId}")]
        public ActionResult<Models.Task> PutTask(long taskId, Models.Task task)
        {
            try
            {
                var userIdClainm = User.Claims.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.InvariantCultureIgnoreCase));

                if (userIdClainm == null)
                {
                    throw new Exception("Token not valid");
                }

                if (taskId != task.TaskId)
                {
                    return BadRequest();
                }

                _taskService.ModifyTask(task, userIdClainm.Value);

                return Ok(task);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        // PUT: api/Tasks/unactivate/5
        [HttpPut("unactivate/{taskId}")]
        public ActionResult<Models.Task> UnactivateTask(long taskId)
        {
            try
            {
                var userIdClainm = User.Claims.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.InvariantCultureIgnoreCase));

                if (userIdClainm == null)
                {
                    throw new Exception("Token not valid");
                }

                Models.Task task = _taskService.GetTaskById(taskId);

                if (task == null)
                {
                    return BadRequest();
                }

                task.TaskIsActivate = 0;

                _taskService.ModifyTask(task, userIdClainm.Value);

                return Ok(task);
            }
            catch (Exception err)
            {
                throw err;
            }
        }


        // PUT: api/Tasks/finish/5
        [HttpPut("finish/{taskId}")]
        public ActionResult<Models.Task> FinishTask(long taskId)
        {
            try
            {
                var userIdClainm = User.Claims.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.InvariantCultureIgnoreCase));

                if (userIdClainm == null)
                {
                    throw new Exception("Token not valid");
                }

                Models.Task task = _taskService.GetTaskById(taskId);

                if (task == null)
                {
                    return BadRequest();
                }

                task.TaskStatus = 1;

                _taskService.ModifyTask(task, userIdClainm.Value);

                return Ok(task);
            }
            catch (Exception err)
            {
                throw err;
            }
        }
    }
}
