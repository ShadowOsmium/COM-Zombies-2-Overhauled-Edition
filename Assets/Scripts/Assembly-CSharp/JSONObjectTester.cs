using System.Collections.Generic;
using Boomlagoon.JSON;
using UnityEngine;

public class JSONObjectTester : MonoBehaviour
{
	public GUIText infoText;

	private string stringToEvaluate = "{\"web-app\": {\r\n  \"servlet\": [   \r\n    {\r\n      \"servlet-name\": \"cofaxCDS\",\r\n      \"servlet-class\": \"org.cofax.cds.CDSServlet\",\r\n      \"init-param\": {\r\n        \"configGlossary:installationAt\": \"Philadelphia, PA\",\r\n        \"configGlossary:adminEmail\": \"ksm@pobox.com\",\r\n        \"configGlossary:poweredBy\": \"Cofax\",\r\n        \"configGlossary:poweredByIcon\": \"/images/cofax.gif\",\r\n        \"configGlossary:staticPath\": \"/content/static\",\r\n        \"templateProcessorClass\": \"org.cofax.WysiwygTemplate\",\r\n        \"templateLoaderClass\": \"org.cofax.FilesTemplateLoader\",\r\n        \"templatePath\": \"templates\",\r\n        \"templateOverridePath\": \"\",\r\n        \"defaultListTemplate\": \"listTemplate.htm\",\r\n        \"defaultFileTemplate\": \"articleTemplate.htm\",\r\n        \"useJSP\": false,\r\n        \"jspListTemplate\": \"listTemplate.jsp\",\r\n        \"jspFileTemplate\": \"articleTemplate.jsp\",\r\n        \"cachePackageTagsTrack\": 200,\r\n        \"cachePackageTagsStore\": 200,\r\n        \"cachePackageTagsRefresh\": 60,\r\n        \"cacheTemplatesTrack\": 100,\r\n        \"cacheTemplatesStore\": 50,\r\n        \"cacheTemplatesRefresh\": 15,\r\n        \"cachePagesTrack\": 200,\r\n        \"cachePagesStore\": 100,\r\n        \"cachePagesRefresh\": 10,\r\n        \"cachePagesDirtyRead\": 10,\r\n        \"searchEngineListTemplate\": \"forSearchEnginesList.htm\",\r\n        \"searchEngineFileTemplate\": \"forSearchEngines.htm\",\r\n        \"searchEngineRobotsDb\": \"WEB-INF/robots.db\",\r\n        \"useDataStore\": true,\r\n        \"dataStoreClass\": \"org.cofax.SqlDataStore\",\r\n        \"redirectionClass\": \"org.cofax.SqlRedirection\",\r\n        \"dataStoreName\": \"cofax\",\r\n        \"dataStoreDriver\": \"com.microsoft.jdbc.sqlserver.SQLServerDriver\",\r\n        \"dataStoreUrl\": \"jdbc:microsoft:sqlserver://LOCALHOST:1433;DatabaseName=goon\",\r\n        \"dataStoreUser\": \"sa\",\r\n        \"dataStorePassword\": \"dataStoreTestQuery\",\r\n        \"dataStoreTestQuery\": \"SET NOCOUNT ON;select test='test';\",\r\n        \"dataStoreLogFile\": \"/usr/local/tomcat/logs/datastore.log\",\r\n        \"dataStoreInitConns\": 10,\r\n        \"dataStoreMaxConns\": 100,\r\n        \"dataStoreConnUsageLimit\": 100,\r\n        \"dataStoreLogLevel\": \"debug\",\r\n        \"maxUrlLength\": 500}},\r\n    {\r\n      \"servlet-name\": \"cofaxEmail\",\r\n      \"servlet-class\": \"org.cofax.cds.EmailServlet\",\r\n      \"init-param\": {\r\n      \"mailHost\": \"mail1\",\r\n      \"mailHostOverride\": \"mail2\"}},\r\n    {\r\n      \"servlet-name\": \"cofaxAdmin\",\r\n      \"servlet-class\": \"org.cofax.cds.AdminServlet\"},\r\n \r\n    {\r\n      \"servlet-name\": \"fileServlet\",\r\n      \"servlet-class\": \"org.cofax.cds.FileServlet\"},\r\n    {\r\n      \"servlet-name\": \"cofaxTools\",\r\n      \"servlet-class\": \"org.cofax.cms.CofaxToolsServlet\",\r\n      \"init-param\": {\r\n        \"templatePath\": \"toolstemplates/\",\r\n        \"log\": 1,\r\n        \"logLocation\": \"/usr/local/tomcat/logs/CofaxTools.log\",\r\n        \"logMaxSize\": \"\",\r\n        \"dataLog\": 1,\r\n        \"dataLogLocation\": \"/usr/local/tomcat/logs/dataLog.log\",\r\n        \"dataLogMaxSize\": \"\",\r\n        \"removePageCache\": \"/content/admin/remove?cache=pages&id=\",\r\n        \"removeTemplateCache\": \"/content/admin/remove?cache=templates&id=\",\r\n        \"fileTransferFolder\": \"/usr/local/tomcat/webapps/content/fileTransferFolder\",\r\n        \"lookInContext\": 1,\r\n        \"adminGroupID\": 4,\r\n        \"betaServer\": true}}],\r\n  \"servlet-mapping\": {\r\n    \"cofaxCDS\": \"/\",\r\n    \"cofaxEmail\": \"/cofaxutil/aemail/*\",\r\n    \"cofaxAdmin\": \"/admin/*\",\r\n    \"fileServlet\": \"/static/*\",\r\n    \"cofaxTools\": \"/tools/*\"},\r\n \r\n  \"taglib\": {\r\n    \"taglib-uri\": \"cofax.tld\",\r\n    \"taglib-location\": \"/WEB-INF/tlds/cofax.tld\"}}}";

	private void Start()
	{
		infoText.gameObject.SetActive(false);
		JSONObject jSONObject = JSONObject.Parse(stringToEvaluate);
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.Add("key", "value");
		jSONObject2.Add("otherKey", 123.0);
		jSONObject2.Add("thirdKey", false);
		jSONObject2.Add("fourthKey", new JSONValue(JSONValueType.Null));
		foreach (KeyValuePair<string, JSONValue> item in jSONObject2)
		{
			Debug.Log("key : value -> " + item.Key + " : " + item.Value);
			Debug.Log("pair.Value.Type.ToString() -> " + item.Value.Type);
			if (item.Value.Type == JSONValueType.Number)
			{
				Debug.Log("Value is a number: " + item.Value.Number);
			}
		}
		JSONObject jSONObject3 = new JSONObject();
		jSONObject3.Add("key", "value");
		jSONObject3.Add("otherKey", 123.0);
		jSONObject3.Add("thirdKey", false);
		JSONObject jSONObject4 = jSONObject3;
		Debug.Log("newObject.ToString() -> " + jSONObject4.ToString());
		Debug.Log("newObject[\"key\"].Str -> " + jSONObject4["key"].Str);
		Debug.Log("newObject.GetValue(\"otherKey\").ToString() -> " + jSONObject4.GetValue("otherKey").ToString());
		Debug.Log("newObject.ContainsKey(\"NotAKey\") -> " + jSONObject4.ContainsKey("NotAKey"));
		jSONObject4.Remove("key");
		Debug.Log("newObject with \"key\" removed: " + jSONObject4.ToString());
		jSONObject4.Clear();
		Debug.Log("newObject cleared: " + jSONObject4.ToString());
	}

	private void OnGUI()
	{
		stringToEvaluate = GUI.TextArea(new Rect(0f, 0f, Screen.width - 300, Screen.height - 5), stringToEvaluate);
		if (GUI.Button(new Rect(Screen.width - 150, 10f, 145f, 75f), "Evaluate JSON"))
		{
			JSONObject jSONObject = JSONObject.Parse(stringToEvaluate);
			if (jSONObject == null)
			{
				Debug.LogError("Failed to parse string, JSONObject == null");
			}
			else
			{
				Debug.Log("Succesfully created JSONObject");
				Debug.Log(jSONObject.ToString());
			}
		}
		if (GUI.Button(new Rect(Screen.width - 150, 95f, 145f, 75f), "Clear textbox"))
		{
			stringToEvaluate = string.Empty;
		}
	}
}
