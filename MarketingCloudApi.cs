namespace SalesforceMarketingCloudIntegration;
public class MarketingCloudApi
{
    //
    private readonly MarketingCloudController _controller;
    private static readonly HttpClient _client = new HttpClient();

    //
    public MarketingCloudApi(MarketingCloudClientConfigObject credntial)
    {
        string _ContentType = "application/json";

        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(_ContentType));

        this._controller = new MarketingCloudController(new MarketingCloudRepo(_client, credntial));
    }

    /// <summary>
    /// Initiate the service
    /// </summary>
    /// <returns></returns>
    public async Task ServiceInit()
    {
        await this._controller.ServiceInit();
    }

    /// <summary>
    /// Insert rows to data extension async
    /// </summary>
    /// <typeparam name="ParamType"></typeparam>
    /// <param name="DeId"></param>
    /// <param name="rows"></param>
    /// <returns>(string) Request Id</returns>
    public async Task<string> InsertRowAsync<ParamType>(string DeId, ParamType rows)
    {
        return await this._controller.InsertRowAsync(DeId, rows);
    }

    /// <summary>
    /// Update rows or insert rows into the data extension async
    /// </summary>
    /// <typeparam name="ParamType"></typeparam>
    /// <param name="DeId"></param>
    /// <param name="rows"></param>
    /// <returns></returns>
    public async Task<string> UpsertRowAsync<ParamType>(string DeId, ParamType rows)
    {
        return await this._controller.UpsertRowAsync(DeId, rows);
    }

    /// <summary>
    /// Get the request status
    /// </summary>
    /// <param name="requestId"></param>
    /// <returns>boolean</returns>
    public async Task<bool> GetStatusOfAsyncRequest(string requestId)
    {
        return await this._controller.GetStatusOfAsyncRequest(requestId);
    }
}

