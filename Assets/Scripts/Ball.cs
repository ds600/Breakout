using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ball : MonoBehaviour
{
    public GameObject spieler;
    public AudioClip kollisionZiegelAudio;
    public Spieler spielerKlasse;
    public int anzahlLeben = 5;
    public int anzahlPunkte = 0;
    Rigidbody2D rb;

    public Text punkteAnzeige;
    public Text lebenAnzeige;
    public Text infoAnzeige;

    public float keinBallKontakt;
    public bool ballkontakt;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        GameObject anderesObjekt = coll.gameObject;

        // Wenn ich mit einem normalen grünen Ziegel zusammenstoße
        if (anderesObjekt.tag == "Ziegel")
        {
            keinBallKontakt = Time.time;
            ballkontakt = true;
            // Audio abspielen (Name der Audiodatei, an Position)
            AudioSource.PlayClipAtPoint(
                kollisionZiegelAudio, transform.position);
            anzahlPunkte++;

            // Punkteanzeige
            punkteAnzeige.text = "Punkte: " + anzahlPunkte;

            if (anzahlPunkte < 50)
            {
                // Objekt mit 0.1 sec. delay zerstören
                Destroy(anderesObjekt, 0.1f);
                // Geschwindigkeit um 10% erhöhen bzw. mal 1.1 nehmen
                if (anzahlPunkte % 10 == 0)
                    rb.velocity = new Vector2(
                        rb.velocity.x * 1.1f, rb.velocity.y * 1.1f);
            }
            // Wenn alle zerstört sind, man also 50 Punkte hat
            else
            {
                // Dann soll der Ball und der Spieler deaktiviert werden 
                Destroy(anderesObjekt);
                spieler.SetActive(false);
                gameObject.SetActive(false);

                // Info Anzeige soll einem den Sieg + Zeit anzeigen
                infoAnzeige.text = string.Format( "Du hast in {0,6:##0.0} sec. gewonnen!", Time.time - spielerKlasse.spielZeitStart);
                float spielZeitAktuell = Time.time - spielerKlasse.spielZeitStart;
                // Wenn die Zeit besser war als die alte, dann soll sie abgespeichert werden
                PlayerPrefs.SetFloat("zeitAlt", spielZeitAktuell);
                PlayerPrefs.Save();
            }
        }
        // "BodenMitte" ist das todes-Zonen Tag
        else if (anderesObjekt.tag == "BodenMitte")
        {
            // Ein Leben abziehen
            anzahlLeben--;

            // Einmal alles resetten
            spieler.SetActive(false);
            gameObject.SetActive(false);
            spielerKlasse.ballUnterwegs = false;

            lebenAnzeige.text = "Leben: " + anzahlLeben;

            // Lebenanzahl sagen, wenn man keine mehr hat, stattdessen eine Loose message
            if (anzahlLeben >= 1)
            {
                Invoke("NaechstesLeben", 1);
                infoAnzeige.text = "Du hast nur noch " + anzahlLeben + " Leben!";

            }
            else
            {
                infoAnzeige.text = "Du hast verloren.";
            }
        }
        // Wenn man den Spieler hitted, dann soll der Ballkontakt resetten. Dieser ist nur wichtig, damit der Ball nicht soft locked wird
        if (anderesObjekt.tag == "Spieler")
        {
            keinBallKontakt = Time.time;
            ballkontakt = true;
        }
        // Wenn ich mit einem gelben Ziegel zusammenstoße, wird er durch einen grünen ersetzt
        if (anderesObjekt.tag == "Ziegel2")
        {
            Destroy(anderesObjekt, 0.1f);
            Instantiate(spielerKlasse.ziegelPrefab, new Vector3(
                    anderesObjekt.transform.position.x,
                    anderesObjekt.transform.position.y, 0), Quaternion.identity);
        }
        // Wenn ich mit einem roten Ziegel zusammenstoße, wird er durch einen gelben ersetzt
        if (anderesObjekt.tag == "Ziegel3")
        {
            Destroy(anderesObjekt, 0.1f);
            Instantiate(spielerKlasse.ziegelPrefab2, new Vector3(
                    anderesObjekt.transform.position.x,
                    anderesObjekt.transform.position.y, 0), Quaternion.identity);
        }

    }

    // Infoanzeigentext wird entfernt und alles wird wieder auf die Grundposition gebracht
    void NaechstesLeben()
    {
        infoAnzeige.text = "";
        spielerKlasse.AufStartpunkt();
    }
}
