
using Assets.Script.Humans;
using UnityEngine;
using UnityEngine.UI;
public class TutorialOutside : MonoBehaviour
{
    Player player;
    Grabber grabber;

    [SerializeField] Image attackTutorial;
    [SerializeField] Image collectTutorial;
    [SerializeField] Image dodgeTutorial;
    [SerializeField] Image grabTutorial;
    [SerializeField] Image throwTutorial;
    private void Awake()
    {
        player = GameManager.Instance.Player.GetComponent<Player>();
        grabber = player.GetComponentInChildren<Grabber>();

    }

    private void OnEnable()
    {
        GameManager.Instance.onCarriedHumansChange += (humans) =>
        {
            if (collectTutorial != null)
            {
                Destroy(collectTutorial.gameObject);
            }
        };
        GameManager.Instance.onCarriedResourcesChange += (resources) =>
        {
            if (collectTutorial != null)
            {
                Destroy(collectTutorial.gameObject);
            }
        };

        grabber.onObjectInRange += () =>
        {
            if (grabTutorial != null)
            {
                grabTutorial.gameObject.SetActive(true);
            }
        };

        grabber.onObjectGrabbed += () =>
        {
            if (grabTutorial != null)
            {
                Destroy(grabTutorial.gameObject);
            }
        };
        grabber.onObjectGrabbed += () =>
        {
            if (throwTutorial != null)
            {
                throwTutorial.gameObject.SetActive(true);
            }
        };
        grabber.onObjectThrown += () =>
        {
            if (throwTutorial != null)
            {
                Destroy(throwTutorial.gameObject);
            }
        };

        if (attackTutorial != null)
            attackTutorial.gameObject.SetActive(true);
        if (dodgeTutorial != null)
            dodgeTutorial.gameObject.SetActive(true);
        if (collectTutorial != null)
            collectTutorial.gameObject.SetActive(true);

        if (transform.childCount < 1) Destroy(gameObject);
    }

    private void OnDisable()
    {
        GameManager.Instance.onCarriedHumansChange -= (humans) =>
        {
            if (collectTutorial != null)
            {
                Destroy(collectTutorial.gameObject);
            }
        };
        GameManager.Instance.onCarriedResourcesChange -= (resources) =>
        {
            if (collectTutorial != null)
            {
                Destroy(collectTutorial.gameObject);
            }
        };
        grabber.onObjectInRange -= () => grabTutorial.gameObject.SetActive(true);
        grabber.onObjectGrabbed -= () => grabTutorial.gameObject.SetActive(false);
        grabber.onObjectGrabbed -= () => throwTutorial.gameObject.SetActive(true);
        grabber.onObjectThrown -= () => throwTutorial.gameObject.SetActive(false);
    }
    void DisableCollectTutorial(EResource resource, int qty)
    {
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) && attackTutorial != null)
        {
            Destroy(attackTutorial.gameObject);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && dodgeTutorial != null)
        {
            Destroy(dodgeTutorial.gameObject);
        }
    }
}
