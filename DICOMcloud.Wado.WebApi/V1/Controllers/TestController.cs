﻿namespace DICOMcloud.Wado.WebApi.Controllers
{
    #region Usings

    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    #endregion

    /// <summary>
    /// The Trading Market Controller.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    // [Authorize]
    public class TestController : ControllerBase
    {
        public TestController(
            IOptions<UrlOptions> options
        )
        {
            var a = "";
        }

        #region Fields

        #endregion

        #region Public Methods And Operators

        /// <summary>
        /// Test
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost("TestMethod")]
        // [Authorize(Policy = "Trading")]
        [ApiConventionMethod(typeof(DefaultApiConventions),
                     nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult> TestMethod()
        {
            // if (exchangeTickers == null || exchangeTickers.Request == null || exchangeTickers.MarketIds == null)
            // {
            //     return NotFound();
            // }
            // return this.Ok(await this._service.ReturnExchangeTickersAsync(exchangeTickers.Request, exchangeTickers.MarketIds));
            return Ok("Test");
        } 

        #endregion
    }
}