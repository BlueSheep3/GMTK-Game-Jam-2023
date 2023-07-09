using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

class ClickableLink : MonoBehaviour
{
    public string linkUrl;

    public void OpenURLOfLink()
    {
        Application.OpenURL(linkUrl);
    }
}
