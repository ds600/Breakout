using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spieler : MonoBehaviour
{
    // Prefabs zur Level Generierung
    public GameObject ziegelPrefab;
    public GameObject ziegelPrefab2;
    public GameObject ziegelPrefab3;

    // Der Ball und sein Script
    public GameObject ball;
    public Ball ballKlasse;

    Rigidbody2D ballRB;
    public bool ballUnterwegs = false;
    float eingabeFaktor = 10;


    public Text infoAnzeige;
    public Text lebenAnzeige;
    public Text punkteAnzeige;
    public Text zeitAnzeige;
    public Text zeitAltAnzeige;

    public bool spielGestartet = false;
    public float spielZeitStart;
    

   


    // Start is called before the first frame update
    void Start()
    {
        ballRB = ball.GetComponent<Rigidbody2D>();
        ZiegelErzeugen();
        ZeitAltLaden();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ///// Wenn der Ball stuck ist  /////
        // Wenn eine gewisse Zeit (10 sek.) vergeht, dann wird der Ball einmal geschupst
        if (ballKlasse.keinBallKontakt - Time.time < (-10) && ballKlasse.ballkontakt == true && spielGestartet)
        {
            ballRB.AddForce(new Vector2(ballRB.velocity.x + 40, ballRB.velocity.y*1.5f));
            ballKlasse.ballkontakt = false;
        }

        if (ballKlasse.keinBallKontakt - Time.time < (-15))
        {
            ballKlasse.ballkontakt = true;

            ballRB.AddForce(new Vector2(ballRB.velocity.x + 40, ballRB.velocity.y * 1.5f));
            ballKlasse.keinBallKontakt = Time.time;
        }
        ///// Wenn der Ball stuck ist  /////

        float xEingabe = Input.GetAxis("Horizontal");
        float yEingabe = Input.GetAxis("Vertical");

        // Wenn der Ball noch nicht fliegt, dann wird ihm ein Stoß mit einer halbwegs random Stärke gegeben
        if (!ballUnterwegs && yEingabe > 0)
        {
            ballRB.AddForce(new Vector2(Random.Range(200, 300) * ((ballKlasse.anzahlPunkte%10) / 100 + 1), Random.Range(200, 300) * ((ballKlasse.anzahlPunkte % 10) / 100 + 1)));
            ballUnterwegs = true;

            // Spiel gestartet auf true, und die Startzeit festhalten
            if (!spielGestartet)
            {
                spielGestartet = true;
                spielZeitStart = Time.time;
            }
            infoAnzeige.text = "";
        }

        // Ja Velocity und Gravity 0 wäre besser, aber das Buch hat es mir anders beigebracht
        if(ballUnterwegs)
        {
            // Je nach eingabe Transform nach links und rechts zur Bewegungsimulation * Time.fixedDeltaTIme um FPS unabhängig zu sein
            // Man sollte nie FixedDeltaTime | FixedUpdate benutzen für movement, da das inputs schluckt, aber ich war damals noch etwas dumm
            float xNeu = transform.position.x +
                xEingabe * eingabeFaktor * Time.fixedDeltaTime;
            if (xNeu < -6) xNeu = -6;
            if (xNeu > 6) xNeu = 6;
            transform.position =
                new Vector3(xNeu, transform.position.y, 0);
        }

        // Zeitanzeige oben links aktualisieren
        if (spielGestartet)
            zeitAnzeige.text = string.Format(
                "Zeit: {0,6:##0.0} sec.", Time.time - spielZeitStart);
    }

    /// <summary>
    /// Alle Ziegel aus Prefabs Instantiaten und platzieren, jeder 4 Gelb, jeder 8 rot
    /// </summary>
    void ZiegelErzeugen()
    {
        // Wechsel soll den Ziegeltyp der geplaced werden soll bestimmen
        int wechsel = 0;
        // Von links nach rechts
        for (int x = 1; x <= 10; x++)
        {
            // Für jedes X von unten nach Oben
            for (int y = 1; y <= 5; y++)
            {
                // Wenn der Block nicht der 4, 8, 12, usw. dann einen Grünen block erstellen
                if (wechsel %4 != 0)
                {
                    Instantiate(ziegelPrefab, new Vector3(
                    x * 1.2f - 6.6f,
                    y * 0.75f - 0.25f, 0), Quaternion.identity);
                }
                // Wenn der Block aus der 4er Reihe, aber nicht aus der 8er Reihe ist, dann einen Gelben Block spawnen
                else if (wechsel % 4 == 0 && wechsel % 8 != 0)
                {
                    Instantiate(ziegelPrefab2, new Vector3(
                    x * 1.2f - 6.6f,
                    y * 0.75f - 0.25f, 0), Quaternion.identity);
                }
                // Sonst einen Roten Block
                else
                {
                    Instantiate(ziegelPrefab3, new Vector3(
                    x * 1.2f - 6.6f,
                    y * 0.75f - 0.25f, 0), Quaternion.identity);
                }
                wechsel++;
            }
        }
        // Wechsel wieder auf 0 setzen, ich glaube actually das diese Zeile unnötig ist
        wechsel = 0;

    }

    /// <summary>
    /// Den Highscore aus PlayerPrefs laden und Anzeigen lassen
    /// </summary>
    void ZeitAltLaden()
    {
        float zeitAlt = 0;
        if (PlayerPrefs.HasKey("zeitAlt"))
            zeitAlt = PlayerPrefs.GetFloat("zeitAlt");
        zeitAltAnzeige.text =
            string.Format("Best: {0,6:##0.0} sec.", zeitAlt);
    }


    /// <summary>
    /// Alle Eigenschaften wieder auf Spielstart zurücksetzen
    /// </summary>
    public void SpielNeuButton_Click()
    {
        // Ja ich weiß jetzt, dass ich genau so gut den Scenemanager nehmen kann um die Scene einfach neu zu laden

        ballKlasse.anzahlPunkte = 0;
        ballKlasse.anzahlLeben = 5;

        punkteAnzeige.text = "Punkte: 0";
        lebenAnzeige.text = "Leben: 5";
        zeitAnzeige.text = "Zeit:    0.0 sec.";
        infoAnzeige.text = "Schieße den Ball mit "
            + "Pfeil-Aufwärts / W ab. \n Bewege den schwarzen "
            + "Spieler mit Pfeil-Links und Pfeil-Rechts / A und D.\n"
            + "Zerstöre grüne Ziegel und vermeide den roten Boden.";

        ballRB.velocity = new Vector2(0, 0);
        ballUnterwegs = false;
        spielZeitStart = Time.time;
        ballKlasse.ballkontakt = true;

        ZiegelEntfernen();

        ZeitAltLaden();
        ZiegelErzeugen();
        AufStartpunkt();
    }

    /// <summary>
    ///  Plattform und Ball auf Startposition zurück positionieren
    /// </summary>
    public void AufStartpunkt()
    {
        gameObject.SetActive(true);
        transform.position = new Vector3(0, -4.55f, 0);

        ball.SetActive(true);
        ball.transform.position = new Vector3(0, -4.1f, 0);
    }


    /// Programm beenden
    public void AnwendungsEndeButton_Click()
    {
        Application.Quit();
    }


    /// <summary>
    /// Alle Ziegel zerstören, wird gebraucht um das Level zu resetten
    /// </summary>
    void ZiegelEntfernen()
    {
        // 3 Arrays, mit allen Ziegeln für jede Art
        GameObject[] alleZiegel = GameObject.FindGameObjectsWithTag("Ziegel");
        GameObject[] alleZiegel2 = GameObject.FindGameObjectsWithTag("Ziegel2");
        GameObject[] alleZiegel3 = GameObject.FindGameObjectsWithTag("Ziegel3");

        // Alle Ziegel aus den Arrays zerstören
        foreach(GameObject element in alleZiegel)
            Destroy(element);
        foreach(GameObject element in alleZiegel2)
            Destroy(element);
        foreach(GameObject element in alleZiegel3)
            Destroy(element);
    }

}

 
