using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    GameRunner game;

    public void ShowText( Color color, string value )
    {
        GameObject text = (GameObject)GameObject.Instantiate(floatingText);
        TextMesh tm = text.GetComponent<TextMesh>();
        text.transform.position = transform.position;
        tm.color = color;
        tm.text = value;
    }

    public GameObject floatingText;

    public enum PowerupType
    {
        Point, CoreHealth, Speed, SuperRepair
    };

    public PowerupType type;

    private void OnTriggerEnter( Collider other )
    {
        PlayerController p = other.gameObject.GetComponent<PlayerController>();

        Debug.Log( "player entered?" );
        if(p != null)
        {
            Debug.Log( "player entered" );
            if(type == PowerupType.Point)
            {
                game.Score += 100;
                ShowText( Color.blue, "Score!" );
            }
            if( type == PowerupType.CoreHealth )
            {
                game.Score += 10;
                ShowText( Color.blue, "Core Health +20!" );
                game.powerCore.CoreHealth += .2f;
            }


            if( type == PowerupType.Speed )
            {
                if( game.player.agent.speed > 7 )
                {
                    game.Score += 50;
                    ShowText( Color.blue, "Speed Maxed! Extra Score!" );
                }
                else
                {
                    game.Score += 10;
                    ShowText( Color.blue, "Speed up!" );
                    game.player.agent.speed += 1;
                }
            }
            if( type == PowerupType.SuperRepair )
            {
                game.Score += 10;
                ShowText( Color.blue, "Repair up!" );
                game.player.repairPower += .01f;
            }

            Destroy( gameObject );
        }
    }

    private void Awake()
    {
        game = GameObject.FindObjectOfType<GameRunner>();
    }
}
