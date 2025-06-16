using UnityEngine;
using UnityEngine.UI;

public class ShipMovement : MonoBehaviour
{
    private string status = "Stopped";
    public Transform target;
    public Button start_btn;
    public Button stop_btn;
    public Button reset_btn;
    public int speed = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Button start = start_btn.GetComponent<Button>();
        Button stop = stop_btn.GetComponent<Button>();
        Button reset = reset_btn.GetComponent<Button>();
        start.onClick.AddListener(onStart);
        stop.onClick.AddListener(onStop);
        reset.onClick.AddListener(onReset);
    }
    void onReset()
    {
        if (target != null)
        {
            Vector3 newPos = new Vector3(0, -100, 0);
            target.position = newPos;
        }
    }
    void onStop()
    {
        status = "Stopped";
    }
    void onStart ()
    {
        status = "Started";
    }
    // Update is called once per frame
    void Update()
    {
        if (target == null) return;
        if (status == "Started")
        {
            Vector3 newPos = new Vector3(1, 0, 0);
            target.position += newPos;
        }
    }
}
