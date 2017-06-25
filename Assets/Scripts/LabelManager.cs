using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.iOS
{
	public class LabelManager : MonoSingleton<LabelManager> {

		public GameObject LabelPrefab;

		private List<ArLabel> m_labels = new List<ArLabel>();

		public ArLabel AddLabel(Vector3 pos, Quaternion rot)
		{
			var item = GameObject.Instantiate(LabelPrefab, pos, rot, transform).GetComponent<ArLabel>();
			item.IsNew = true;
			item.ForceToSelectedView();
			InputManager.Instance.SelectItem(item.GetComponent<ISelectable>());
			m_labels.Add(item);
			return item;
		}

		public void RemoveLabel(GameObject toRemove)
		{
			m_labels.Remove(toRemove.GetComponent<ArLabel>());
			Destroy(toRemove);
			InputManager.Instance.SelectItem(null);
		}

		public void TranslateAll(string toLangCode)
		{
			for (int i = 0; i < m_labels.Count; i++)
			{
				var lbl = m_labels[i];
				Translate.RequestTranslation(new Translate.TranslateArgs()
					{
						sourceLang = "en",
						targetLang = toLangCode,
						toTranslate = lbl.GetText("en")
					},
					(string s) => 
					{
						lbl.SetText(toLangCode, s);
						lbl.UpdateActiveTranslation();
					});
			}
		}
	}
}