using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniJSON;

public class Translate : MonoBehaviour {
	
	public bool isDebug = false;

	private static Translate Instance;
	private string translatedText = "";

	void Awake()
	{
		Instance = this;
	}

	void Start () 
	{
		// Strictly for debugging to test a few words!
		if(isDebug)
			StartCoroutine (Process ("en","Bonsoir."));
	}

	public class TranslateArgs
	{
		public string sourceLang = "en";
		public string targetLang = "es";
		public string toTranslate = "hello";
	};

	public static void RequestTranslation(TranslateArgs args, System.Action<string> onTranslated)
	{
		Instance.StartCoroutine (Process (args.sourceLang, args.targetLang, args.toTranslate, onTranslated));
	}

	// We have use googles own api built into google Translator.
	public IEnumerator Process (string targetLang, string sourceText) {
		// We use Auto by default to determine if google can figure it out.. sometimes it can't.
		string sourceLang = "auto";
		// Construct the url using our variables and googles api.
		string url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl=" 
			+ sourceLang + "&tl=" + targetLang + "&dt=t&q=" + WWW.EscapeURL(sourceText);

		// Put together our unity bits for the web call.
		WWW www = new WWW (url);
		// Now we actually make the call and wait for it to finish.
		yield return www;

		// Check to see if it's done.
		if (www.isDone) {
			// Check to see if we don't have any errors.
			if(string.IsNullOrEmpty(www.error)){
				// Parse the response using JSON.
//				IDictionary dict = (IDictionary)Json.Deserialize(www.text);
				System.Object obj = Json.Deserialize(www.text) as System.Object;
				var list0 = (IList)obj;
				var list1 = (IList)list0[0];
				var list2 = (IList)list1[0];
				var str = (string)list2[0];
				 
				// Dig through and take apart the text to get to the good stuff.
//				translatedText = N[0][0][0];
				// This is purely for debugging in the Editor to see if it's the word you wanted.
				if(isDebug)
					print(translatedText);
			}
		}
	}

	// Exactly the same as above but allow the user to change from Auto, for when google get's all Jerk Butt-y
	static IEnumerator Process (string sourceLang,
								string targetLang, 
								string sourceText,
								System.Action<string> onTranslated) 
	{
		string url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl=" 
			+ sourceLang + "&tl=" + targetLang + "&dt=t&q=" + WWW.EscapeURL(sourceText);

		WWW www = new WWW (url);
		yield return www;

		if (www.isDone) 
		{
			if(string.IsNullOrEmpty(www.error))
			{
				System.Object obj = Json.Deserialize(www.text) as System.Object;
				var list0 = (IList)obj;
				var list1 = (IList)list0[0];
				var list2 = (IList)list1[0];
				onTranslated((string)list2[0]);
			}
		}
	}
}
