using SalesforceMarketingCloudIntegration.DI;
using SalesforceMarketingCloudIntegration.Models;

namespace SalesforceMarketingCloudIntegration;

public class MarketingCloudApi: IMarketingCloudInterface
{
    //
    private readonly MarketingCloudController _controller;

    //
    private static readonly HttpClient _client = new HttpClient();

    /// <summary>
    /// Marketing cloud api wrapper
    /// </summary>
    /// <param name="credntial"></param>
    public MarketingCloudApi(MarketingCloudClientConfigObject credntial)
    {
        //  prepare http client
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        //  set controller
        //  and repo takes http client and marketing cloud credential
        _controller = new MarketingCloudController(new MarketingCloudRepo(_client, credntial));
    }

    /// <summary>
    /// Insert rows to data extension async
    /// </summary>
    /// <typeparam name="ParamType"></typeparam>
    /// <param name="DeId"></param>
    /// <param name="rows"></param>
    /// <returns>(string) Request Id</returns>
    public async Task<string> InsertRowAsync<ParamType>(string DeId, ParamType rows) => await this._controller.InsertRowAsync(DeId, rows);

    /// <summary>
    /// Update rows or insert rows into the data extension async
    /// </summary>
    /// <typeparam name="ParamType"></typeparam>
    /// <param name="DeId"></param>
    /// <param name="rows"></param>
    /// <returns></returns>
    public async Task<string> UpsertRowAsync<ParamType>(string DeId, ParamType rows) => await this._controller.UpsertRowAsync(DeId, rows);

    /// <summary>
    /// Get the request status
    /// </summary>
    /// <param name="requestId"></param>
    /// <returns>boolean</returns>
    public async Task<bool> GetStatusOfAsyncRequest(string requestId) => await this._controller.GetStatusOfAsyncRequest(requestId);

    /// <summary>
    /// Get the result of request
    /// </summary>
    /// <param name="requestId"></param>
    /// <returns></returns>
    public async Task<List<ResultItem>> GetResultOfAsyncRequest(string requestId) => await this._controller.GetResultOfAsyncRequest(requestId);
}

