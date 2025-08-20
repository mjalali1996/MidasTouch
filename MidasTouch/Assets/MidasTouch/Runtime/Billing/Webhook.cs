using System.Text;
using System.Threading.Tasks;
using MidasTouch.Billing.Models;
using UnityEngine;
using UnityEngine.Networking;

namespace MidasTouch.Billing
{
    internal static class Webhook
    {
        internal static async Task<bool> Consume(string url, string market, string itemId, string purchaseToken,
            ItemType type)
        {
            using var webRequest = new UnityWebRequest(url, "POST");

            var obj = new PurchaseDTO
            {
                Market = market,
                ItemId = itemId,
                PurchaseToken = purchaseToken,
                Type = type
            };

            var jsonPayload = JsonUtility.ToJson(obj);

            var bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);

            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            await webRequest.SendWebRequest();

            if (webRequest.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                // Log any errors that occurred.
                Debug.LogError($"Error: {webRequest.error}");
                return false;
            }

            var jsonResponse = webRequest.downloadHandler.text;
            Debug.Log($"Received JSON: {jsonResponse}");
            var webhookResponse = JsonUtility.FromJson<Response>(jsonResponse);

            if (!webhookResponse.Success)
                Debug.LogError(webhookResponse.Message);

            return webhookResponse.Success;
        }
    }
}