using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(TextMeshProUGUI))]
public class Credits : MonoBehaviour, IPointerClickHandler
{
    TMP_Text pTextMeshPro;
    Camera _camera;
    string content = @"
<size=200%><color=yellow>Models</color></size>
<u>
<link=""https://poly.pizza/m/cW3zvvkMOM"">Tank model by ""Quaternius""</link>

<link=""https://creazilla.com/nodes/1403691-buildings-pack-3d-model"">Buildings Pack 3D Model model by ""Quaternius""</link>

<link=""https://www.kenney.nl/assets/blaster-kit"">Blaster Kit by ""Kenney""</link>
</u>

<size=200%><color=yellow>Sounds</color></size>
<u>
<link=""https://freesound.org/people/XHALE303/sounds/535592/"">Freesound - ""BOOM IMPACT 2"" by XHALE303</link>

<link=""https://freesound.org/people/tcpp/sounds/105025/"">Freesound - ""fireM.wav"" by tcpp</link>

<link=""https://freesound.org/people/Isaac200000/sounds/184651/"">Freesound - ""Cannon5.wav"" by Isaac200000</link>

<link=""https://freesound.org/people/Department64/sounds/554749/"">Freesound - ""Electric_Arc_Phased_1"" by Department64</link>

<link=""https://freesound.org/people/Sergenious/sounds/55845/"">Freesound - ""pushbutn.wav"" by Sergenious</link>

<link=""https://freesound.org/people/SilverIllusionist/sounds/580812/"">Freesound - ""Craft Item 2"" by SilverIllusionist</link>

<link=""https://freesound.org/people/Raclure/sounds/483598/"">Freesound - ""Wrong"" by Raclure</link>

<link=""https://freesound.org/people/MATRIXXX_/sounds/402767/"">Freesound - ""Retro, Coin 03.wav"" by MATRIXXX_</link>

<link=""https://freesound.org/people/Kneeling/sounds/448002/"">Freesound - ""cannon.mp3"" by Kneeling</link>
</u>

<size=200%><color=yellow>Images</color></size>
<u>
<link=""https://www.flaticon.com/free-icons/gear"">Gear icons created by Design Circle - Flaticon</link>
</u>
";
    [SerializeField] RectTransform _container;
    RectTransform _textTransform;

    void Awake()
    {
        _camera = Camera.main;
        pTextMeshPro = GetComponent<TMP_Text>();
        _textTransform = GetComponent<RectTransform>();
        pTextMeshPro.text = content;
        _container.sizeDelta = new Vector2(0, pTextMeshPro.preferredHeight);
        _textTransform.sizeDelta = new Vector2(0, pTextMeshPro.preferredHeight);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(pTextMeshPro, Mouse.current.position.value, null);
        if (linkIndex != -1)
        { // was a link clicked?
            TMP_LinkInfo linkInfo = pTextMeshPro.textInfo.linkInfo[linkIndex];

            // open the link id as a url, which is the metadata we added in the text field
            Application.OpenURL(linkInfo.GetLinkID());
        }
    }
}
