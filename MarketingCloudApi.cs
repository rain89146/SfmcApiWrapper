using SalesforceMarketingCloudIntegration.Controller;
using SalesforceMarketingCloudIntegration.DI;
using SalesforceMarketingCloudIntegration.DI.Controller;
using SalesforceMarketingCloudIntegration.Models;
using SalesforceMarketingCloudIntegration.Repo;

namespace SalesforceMarketingCloudIntegration;

public class MarketingCloudApi: IMarketingCloudInterface
{
    //
    private readonly IDataExtensionControllerInterface _deController;
    private readonly IJourneyControllerInterface _journeyController;
    private readonly IContactControllerInterface _contactController;

    //
    private static readonly HttpClient _client = new ();

    /// <summary>
    /// Marketing cloud api wrapper
    /// </summary>
    /// <param name="credential"></param>
    public MarketingCloudApi(MarketingCloudClientConfigObject credential)
    {
        //  prepare http client
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        //  set controller
        //  and repo takes http client and marketing cloud credential

        //  data extension
        _deController = new DataExtensionController(new DataExtensionRepo(_client, credential));

        //  journey
        _journeyController = new JourneyController(new JourneyRepo(_client, credential));

        //  contact
        _contactController = new ContactController(new ContactRepo(_client, credential));

    }

    /// <summary>
    /// Insert rows to data extension async
    /// </summary>
    /// <typeparam name="ParamType"></typeparam>
    /// <param name="DeId"></param>
    /// <param name="rows"></param>
    /// <returns>(string) Request Id</returns>
    public async Task<string> InsertRowAsync<ParamType>(string DeId, ParamType rows) => await this._deController.InsertRowAsync(DeId, rows);

    /// <summary>
    /// Update rows or insert rows into the data extension async
    /// </summary>
    /// <typeparam name="ParamType"></typeparam>
    /// <param name="DeId"></param>
    /// <param name="rows"></param>
    /// <returns></returns>
    public async Task<string> UpsertRowAsync<ParamType>(string DeId, ParamType rows) => await this._deController.UpsertRowAsync(DeId, rows);

    /// <summary>
    /// Get the request status
    /// </summary>
    /// <param name="requestId"></param>
    /// <returns>boolean</returns>
    public async Task<bool> GetStatusOfAsyncRequest(string requestId) => await this._deController.GetStatusOfAsyncRequest(requestId);

    /// <summary>
    /// Get the result of request
    /// </summary>
    /// <param name="requestId"></param>
    /// <returns></returns>
    public async Task<List<ResultItem>> GetResultOfAsyncRequest(string requestId) => await this._deController.GetResultOfAsyncRequest(requestId);

    /// <summary>
    /// Validate email address
    /// </summary>
    /// <param name="emailAddress"></param>
    /// <returns></returns>
    public async Task<bool> ValidateEmailAddress(string emailAddress) => await this._contactController.ValidateEmailAddress(emailAddress);

    /// <summary>
    /// Fire a journey event
    /// </summary>
    /// <param name="contactKey"></param>
    /// <param name="eventDefinitionKey"></param>
    /// <param name="eventData"></param>
    /// <returns></returns>
    public async Task<string> FireJourneyEvent(string contactKey, string eventDefinitionKey, Dictionary<string, string> eventData) => await this._journeyController.FireJourneyEvent(contactKey, eventDefinitionKey, eventData);
}

