using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class MyPlayer : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    private RaycastHit rayHit = new RaycastHit();
    private EventSystem eSystem;

    private Animator animator;

    private Vector3 lastPos;

    private List<int> animHash = new List<int>();

    private bool isInsideStore = false;
    private bool isInsideEnhance = false;
    private bool isVillageHead = false;

    private void Awake()
    {
        eSystem = TownManager.Instance.E_System;

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        TownManager.Instance.FreeLook.Follow = transform;
        TownManager.Instance.FreeLook.LookAt = transform;

        TownManager.Instance.FreeLook.gameObject.SetActive(true);

        lastPos = transform.position;

        animHash.Add(Constants.TownPlayerAnim1);
        animHash.Add(Constants.TownPlayerAnim2);
        animHash.Add(Constants.TownPlayerAnim3);
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(eSystem.IsPointerOverGameObject()) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out rayHit))
            {
                agent.SetDestination(rayHit.point);
            }
        }
        if(Input.GetKeyDown(KeyCode.I) && !TownManager.Instance.UIStore.gameObject.activeSelf && !TownManager.Instance.UIRank.gameObject.activeSelf && !TownManager.Instance.UIEnhance.gameObject.activeSelf)
        {
            bool check = TownManager.Instance.UIInventory.gameObject.activeSelf;
            InventoryUI(!check);
            TownManager.Instance.UIInventory.gameObject.SetActive(!check);
            return;
        }
        if(Input.GetKeyDown(KeyCode.F) && isInsideStore && !TownManager.Instance.UIInventory.gameObject.activeSelf)
        {
            ToggleStoreUI();
            return;
        }
        if (Input.GetKeyDown(KeyCode.F) && isInsideEnhance && !TownManager.Instance.UIInventory.gameObject.activeSelf)
        {
            ToggleEnhanceUI();
        }
        else if(Input.GetKeyDown(KeyCode.F) && isVillageHead && !TownManager.Instance.UIInventory.gameObject.activeSelf)
        {
            ToggleRank();
            return;
        }
        CheckMove();
    }

    public void ToggleRank()
    {
        bool isActive = TownManager.Instance.UIRank.gameObject.activeSelf;

        if(!isActive)
        {
            C_ViewRankPoint packet = new C_ViewRankPoint();
            GameManager.Network.Send(packet);
        }

        TownManager.Instance.UIRank.gameObject.SetActive(!isActive);
    }

    public void AnimationExecute(int animIdx)
    {
        int animKey = animHash[animIdx];
        agent.SetDestination(transform.position);

        C_Animation animationPacket = new C_Animation { AnimCode = animKey };
        GameManager.Network.Send(animationPacket);
    }

    void CheckMove()
    {
        float dist = Vector3.Distance(lastPos, transform.position);
        animator.SetFloat(Constants.TownPlayerMove, dist * 100);

        if (dist > 0.01f)
        {
            TransformInfo tr = new TransformInfo();
            tr.PosX = transform.position.x;
            tr.PosY = transform.position.y;
            tr.PosZ = transform.position.z;
            tr.Rot = transform.eulerAngles.y;

            C_Move enterPacket = new C_Move { Transform = tr };
            GameManager.Network.Send(enterPacket);
        }

        lastPos = transform.position;
    }

    void InventoryUI(bool check)
    {
        if (!check) return;
        C_InventoryViewRequest packet = new C_InventoryViewRequest();
        GameManager.Network.Send(packet);
    }

    public void StoreUI(bool check)
    {
        if(!check) return;
        C_OpenStoreRequest packet = new C_OpenStoreRequest();
        GameManager.Network.Send(packet);
    }
    public void EnhanceUI(bool check)
    {
        if(!check) return;
        C_EnhanceUiRequest packet = new C_EnhanceUiRequest();
        GameManager.Network.Send(packet);
    }

    public void BossMatching(bool check)
    {
        // if (!check) return;
        C_BossMatch packet = new C_BossMatch{ IsIn = check };
        GameManager.Network.Send(packet);
    }

    private void ToggleStoreUI()
    {
        bool isActive = TownManager.Instance.UIStore.gameObject.activeSelf;

        if(!isActive)
        {
            C_OpenStoreRequest packet = new C_OpenStoreRequest();
            GameManager.Network.Send(packet);
        }

        TownManager.Instance.UIStore.gameObject.SetActive(!isActive);
    }
    private void ToggleEnhanceUI()
    {
        bool isActive = TownManager.Instance.UIEnhance.gameObject.activeSelf;

        if (!isActive)
        {
            C_EnhanceUiRequest packet = new C_EnhanceUiRequest();
            GameManager.Network.Send(packet);
        }

        TownManager.Instance.UIEnhance.gameObject.SetActive(!isActive);
    }

    private void BossMatchingUI()
    {
        bool isActive = TownManager.Instance.UIBossMatching.gameObject.activeSelf;

        TownManager.Instance.UIBossMatching.gameObject.SetActive(!isActive);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Store"))
        {
            isInsideStore = true;
        }
        else if (other.CompareTag("Enhance"))
        {
            isInsideEnhance = true;
        }
        else if(other.CompareTag("VillageHead"))
        {
            isVillageHead = true;
        }
        else if(other.CompareTag("Boss"))
        {
            BossMatching(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Store"))
        {
            isInsideStore = false;
        }
        else if(other.CompareTag("Enhance"))
        {
            isInsideEnhance = false;
        }
        else if(other.CompareTag("VillageHead"))
        {
            isVillageHead = false;
        }
        else if (other.CompareTag("Boss"))
        {
            BossMatching(false);
        }
    }
}
