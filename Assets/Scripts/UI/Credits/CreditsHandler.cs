using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CreditsHandler : MonoBehaviour
{
    // based on logic from https://www.youtube.com/watch?v=_kWvZ7_m78U&t=1538s 

    [SerializeField] GameObject background;
    [SerializeField] string path = "Assets/Scripts/UI/Credits/Credits.txt";
    [SerializeField] Font font;
    [SerializeField] Color headerColor = Color.red;
    [SerializeField] Color nameColor = Color.white;
    [SerializeField] int headerSize = 35;
    [SerializeField] int nameSize = 25;
    [SerializeField] float scrollSpeed = 150;
    [SerializeField] int screenSpaceInDivision = 8;

    List<string> headers = new List<string>();
    List<List<string>> titles = new List<List<string>>();
    List<GameObject> creditTexts = new List<GameObject>();
    bool isEnded = false;

    public event EventHandler<GenericEventArgs<bool>> CreditsEnded;

    private void StartCredits()
    {
        background.SetActive(true);
        var lastPosition = new Vector3(Screen.width * .5f, 0, 0);
        for (var i = 0; i < headers.Count; i++)
        {
            var headerTextGameObject = NewTextGameObject(headers[i], true);
            var nextPosition = new Vector3(Screen.width * 0.5f, lastPosition.y - (Screen.height / screenSpaceInDivision), 0);
            headerTextGameObject.transform.position = nextPosition;
            lastPosition = nextPosition;
            creditTexts.Add(headerTextGameObject);
            for (var j = 0; j < titles[i].Count; j++)
            {
                nextPosition = new Vector3(Screen.width * 0.5f, lastPosition.y - (Screen.height / screenSpaceInDivision), 0);
                var textGameObject = NewTextGameObject(titles[i][j], false);
                textGameObject.transform.position = nextPosition;
                lastPosition = nextPosition;
                creditTexts.Add(textGameObject);
            }
        }
        isEnded = false;
    }

    private void Awake()
    {
        var creditsLines = _credits.Split(Environment.NewLine);
        foreach (var currentLine in creditsLines)
        {
            if (string.IsNullOrEmpty(currentLine))
            {
                continue;
            }
            var firstCharacter = currentLine.Substring(0, 1);
            var isIgnore = firstCharacter == "#" || firstCharacter == " " || firstCharacter == "\t" || firstCharacter == string.Empty;
            var isHeader = firstCharacter == "!";
            if (isIgnore)
            {
                continue;
            }
            if (isHeader)
            {
                headers.Add(currentLine.Substring(1));
                titles.Add(new List<string>());
            }
            else
            {
                titles[titles.Count - 1].Add(currentLine);
            }
        }
    }

    public void OnStartCredits()
    {
        creditTexts.Clear();
        StartCredits();
    }

    private GameObject NewTextGameObject(string labelText, bool isHeader)
    {
        var textGameObject = new GameObject(labelText);
        textGameObject.transform.SetParent(transform);
        var textComponent = textGameObject.AddComponent<Text>();
        textComponent.text = labelText;
        textComponent.font = font;
        textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
        textComponent.alignment = TextAnchor.MiddleCenter;
        if (isHeader)
        {
            textComponent.fontStyle = FontStyle.Bold;
            textComponent.color = headerColor;
            textComponent.fontSize = headerSize;
        }
        else
        {
            textComponent.color = nameColor;
            textComponent.fontSize = nameSize;
        }
        textGameObject.transform.localScale = Vector3.one;
        textGameObject.transform.position = new Vector3(Screen.width * .5f, Screen.height * .25f, 0);
        return textGameObject;
    }

    private void Update()
    {
        foreach (var text in creditTexts)
        {
            if (text == null) continue;

            text.transform.position = new Vector3(text.transform.position.x, text.transform.position.y + scrollSpeed * Time.deltaTime, 0);
            if (text.transform.position.y > Screen.height * 1.2f)
            {
                Destroy(text);
            }
        }
        creditTexts.RemoveAll(x => x.transform.position.y > Screen.height * 1.2f);
        if (!isEnded && creditTexts.Count == 0)
        {
            isEnded = true;
            background.SetActive(false);
            CreditsEnded?.Invoke(sender: this, new GenericEventArgs<bool>(true));
        }
    }

    // hacky but works. Ideally would read from a file but file paths get wonky when in builds
    private const string _credits = @"
# Magic Mayhem Credits
# # - denote comments, ! - denote headers

!Magic Mayhem

!Game Designers and Developers

David Fuentes
Josh Shigemitsu
Moises Castro
Noah Wood

!Assets Used

Mixamo : Locomotion Animations + Paladin model
Enkarra (Sketchfab) : Stylized Wizard Hat
Coin and Fireball: https://assetstore.unity.com/packages/vfx/particles/cartoon-fx-free-pack-169179
Collect Coin Sound : https://pixabay.com/sound-effects/collectcoin-6075/
Synty : Dungeon, Enemy, Hero assets, Enviroment
Priest Healing Aura Loop by EminYILDIRIM -- https://freesound.org/s/560254/ -- License: Attribution 4.0
Fireball Explosion.wav by HighPixel -- https://freesound.org/s/431174/ -- License: Creative Commons 0
Healing/Damage Aura and Shield: https://assetstore.unity.com/packages/vfx/particles/hyper-casual-fx-200333
Crystal low poly: https://assetstore.unity.com/packages/3d/props/props-3d-221035
Freeze Cast by JustInvoke: https://freesound.org/people/JustInvoke/sounds/446144/
Elemental Magic Spell Cast A Short by RescopicSound: https://freesound.org/people/RescopicSound/sounds/749795/
Chain Lightning Projectile https://assetstore.unity.com/packages/vfx/particles/cartoon-fx-remaster-free-109565
Chain Lightning Connector: https://assetstore.unity.com/packages/vfx/particles/fx-lightning-ii-free-25210#description
Clay Pot Break Sound : https://freesound.org/people/Kinoton/sounds/399080/
Health Potion: https://assetstore.unity.com/packages/3d/props/potions-coin-and-box-of-pandora-pack-71778
Health Potion Pickup Sound: https://freesound.org/people/IENBA/sounds/698768/
Breakable Barrel: https://assetstore.unity.com/packages/3d/props/3d-low-poly-tools-weapons-containers-274127
Breakable Barrel Sound: https://freesound.org/people/kevinkace/sounds/66772/
MeteorStrike and IceCone Spell: https://assetstore.unity.com/packages/vfx/particles/spells/aoe-magic-spells-vol-1-133012
Earth Shatter : https://assetstore.unity.com/packages/vfx/particles/particle-pack-127325
Earth Shatter Sound : https://freesound.org/people/uagadugu/sounds/222521/
Earth Shatter Sound : https://freesound.org/people/NeoSpica/sounds/512243/
Free Fire VFX + Sound: https://assetstore.unity.com/packages/vfx/particles/fire-explosions/free-fire-vfx-266227
Trap Spike Activate Sound: https://pixabay.com/sound-effects/steel-blade-slice-2-188214/
Prime Trap Sound: https://pixabay.com/sound-effects/mechanical1-107614/
Player Shield Hit Sound: https://freesound.org/people/CTCollab/sounds/223630/
Player Hard Hit Sound: https://freesound.org/people/crunchymaniac/sounds/678424/
Player Death Sound: https://freesound.org/people/SoundBiterSFX/sounds/731506/
Goblin Hit Sound: https://freesound.org/people/HydraSound/sounds/736274/
Goblin Death Sound: https://freesound.org/people/Fenodyrie/sounds/565928/
War Chief Hit/Death Sound: https://freesound.org/people/efectirijillo/sounds/241979/
Golem Hit Sound: https://freesound.org/people/lolamadeus/sounds/179365/
Golem Death Sound: https://freesound.org/people/Artninja/sounds/750822/
Skeleton Hit Sound: https://freesound.org/people/cribbler/sounds/381859/
Skeleton Death Sound: https://freesound.org/people/spookymodem/sounds/202091/

!Thank You For Playing!
";
}
