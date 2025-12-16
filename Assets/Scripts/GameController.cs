using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject player;
    public GameObject mainCamera;
    public GameObject canvas;
    public GameObject encounters;

    public WorldGen worldGen;

    public AudioSource audioSource;

    public AudioClip key; 
    public AudioClip good;
    public AudioClip bad;


    int dialogIndex = 0;
    string[] textlist = new string[] { };
    string encounterType = "";
    bool dialogProgress = false;
    bool dialogActive = false;

    public int friendlyRations;
    public int hostileRations;
    public int terrainRations;

    public string[] menuLines = { "" };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            worldGen.GenerateMap(player, mainCamera, encounters);
            Encounter("menu", menuLines);
        }
    }

    private void FixedUpdate()
    {
        if (dialogActive)
        {
            if (dialogIndex <= textlist.Length)
            {
                if (!dialogProgress && Keyboard.current.enterKey.IsActuated())
                {
                    dialogIndex++;
                    if (dialogIndex >= textlist.Length)
                    {
                        return;
                    }
                    canvas.transform.GetChild(9).gameObject.GetComponent<TextMeshProUGUI>().text = textlist[dialogIndex];
                    dialogProgress = true;
                } else if (dialogProgress && !Keyboard.current.enterKey.IsActuated())
                {
                    dialogProgress = false;
                }
            }
            else
            {
                switch (encounterType)
                {
                    case "menu":
                        canvas.transform.GetChild(0).gameObject.SetActive(false);

                        canvas.transform.GetChild(7).gameObject.SetActive(true);
                        canvas.transform.GetChild(8).gameObject.SetActive(true);
                        break;
                    case "friendly":
                        canvas.transform.GetChild(1).gameObject.SetActive(false);
                        player.GetComponent<PlayerController>().rations += friendlyRations;
                        audioSource.PlayOneShot(key);
                        break;
                    case "hostile":
                        canvas.transform.GetChild(2).gameObject.SetActive(false);
                        player.GetComponent<PlayerController>().rations -= hostileRations;
                        break;
                    case "terrain":
                        canvas.transform.GetChild(3).gameObject.SetActive(false);
                        player.GetComponent<PlayerController>().rations -= terrainRations;
                        break;
                    case "lose":
                        if (SceneManager.GetActiveScene().name == "MainScene")
                            SceneManager.LoadScene("MainScene");
                        else
                        {
                            SceneManager.LoadScene("TownScene");
                        }
                        break;
                    case "town":
                        SceneManager.LoadScene("TownScene");
                        break;
                    case "win":
                        SceneManager.LoadScene("MainScene");
                        break;
                }

                canvas.transform.GetChild(9).gameObject.GetComponent<TextMeshProUGUI>().text = "";

                player.GetComponent<PlayerController>().moveable = true;
                dialogActive = false;
                Debug.Log("Encounter ended. Index: " + dialogIndex + " textLength: " + textlist.Length);
            }
        }
        canvas.transform.GetChild(8).gameObject.GetComponent<TextMeshProUGUI>().text = $"x {player.GetComponent<PlayerController>().rations}";
    }

    public void Encounter(string type, string[] text)
    {
        player.GetComponent<PlayerController>().moveable = false;
        dialogIndex = 0;
        dialogProgress = true;
        encounterType = type;
        textlist = text;

        switch (encounterType)
        {
            case "menu":
                canvas.transform.GetChild(0).gameObject.SetActive(true);
                audioSource.PlayOneShot(good);
                break;
            case "friendly":
                canvas.transform.GetChild(1).gameObject.SetActive(true);
                audioSource.PlayOneShot(good);
                break;
            case "hostile":
                canvas.transform.GetChild(2).gameObject.SetActive(true);
                audioSource.PlayOneShot(bad);
                break;
            case "terrain":
                canvas.transform.GetChild(3).gameObject.SetActive(true);
                audioSource.PlayOneShot(bad);
                break;
            case "lose":
                canvas.transform.GetChild(4).gameObject.SetActive(true);
                audioSource.PlayOneShot(bad);
                break;
            case "town":
                canvas.transform.GetChild(5).gameObject.SetActive(true);
                audioSource.PlayOneShot(good);
                break;
            case "win":
                canvas.transform.GetChild(6).gameObject.SetActive(true);
                audioSource.PlayOneShot(good);
                break;
        }

        canvas.transform.GetChild(9).gameObject.GetComponent<TextMeshProUGUI>().text = textlist[0];
        dialogActive = true;
        Debug.Log("Encounter started: " + encounterType);
    }
}
