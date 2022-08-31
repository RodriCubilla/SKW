using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class follow_playerESO : MonoBehaviour
{
    public GameObject player;
    public Vector3 c_velocity;
    float smoot_time = .15f;
    Vector3 min_position;
    bool pixel_perfect = true;
    Vector2 pixel_bufer_position;
    public Vector2 max_position; 

    // Use this for initialization
    void Start()
    {
        min_position = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //calculo un movimiento elastico hacia el player
        Vector3 newpos = transform.position;
        newpos += (Vector3)pixel_bufer_position;
        newpos = Vector3.SmoothDamp(transform.position, player.transform.position, ref c_velocity, smoot_time);

        //Movimiento en X
        if (newpos.x < min_position.x)
        {
            newpos.x = min_position.x;
            pixel_bufer_position = Vector2.zero;
        }
        if (newpos.x > max_position.x)
        {
            newpos.x = max_position.x;
            pixel_bufer_position = Vector2.zero;
        }

        //Movimiento en Y
        if (newpos.y < min_position.y)
        {
            newpos.y = min_position.y;
            pixel_bufer_position = Vector2.zero;
        }
        if (newpos.y > max_position.y)
        {
            newpos.y = max_position.y;
            pixel_bufer_position = Vector2.zero;
        }

        transform.position = pixel_perfect_position(newpos);     //muevo la camara a la posicion indicada

    }

    Vector3 pixel_perfect_position(Vector3 _posi)
    {
        if (pixel_perfect) //check if the current camera position is a pixel perfect position and saves a buffer for soft movement!
        {
            //_posi += (Vector3)pixel_bufer_position;
            float min_movement_x = (camera_width * 2) / Screen.width;
            float min_movement_y = (camera_height * 2) / Screen.height;
          
            pixel_bufer_position.x = _posi.x % min_movement_x;
            pixel_bufer_position.y = _posi.y % min_movement_y;

            if (pixel_bufer_position.x != 0)
            {
                _posi.x -= pixel_bufer_position.x > .5f ? -pixel_bufer_position.x : pixel_bufer_position.x;
            }

            if (pixel_bufer_position.y != 0)
            {
                _posi.y -= pixel_bufer_position.y > .5f ? -pixel_bufer_position.y : pixel_bufer_position.y;
            }

            transform.localPosition = _posi;
        }

        return _posi;
    }

    public static float camera_width
    {
        get
        {
            return Camera.main.orthographicSize * Camera.main.aspect;
        }
    }

    public static float camera_height
    {
        get
        {
            return Camera.main.orthographicSize;
        }
    }
}
