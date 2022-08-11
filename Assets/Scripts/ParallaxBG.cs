using UnityEngine;
using UnityEngine.UI;
public class ParallaxBG : MonoBehaviour
{
    Vector2 _starPos;
    [SerializeField] int _moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        _starPos = transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        Vector2 pz = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        float posX = Mathf.Lerp(transform.position.x, _starPos.x + (pz.x * _moveSpeed), 1.5f * Time.deltaTime);
        float posY = Mathf.Lerp(transform.position.y, _starPos.y + (pz.y * _moveSpeed), 1.5f * Time.deltaTime);

        transform.position = new Vector3(posX,posY,0);
    }
}
