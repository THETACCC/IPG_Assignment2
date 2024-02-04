using SKCell;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Block : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    //Mouse behaviours
    private bool mouse_over, prev_mouseOver;
    public bool mouse_drag;
    private Vector3 drag_offset, drag_start_pos;
    public Vector3 moveposition = Vector3.zero;
    public bool canDrag = false;

    //mouse position
    private Vector3 mouseposition;

    private Vector3 cpos;
    private Vector3 npos;

    private Vector3 oScale;

    void Start()
    {
        oScale = transform.localScale;
    }

    void Update()
    {
        if(gameManager.state == GameState.Placing)
        {
            UpdateMouseBehavior();
        }


    }

    public void SyncLocalScale(Vector3 scale)
    {
        oScale = scale;
    }


    private void UpdateMouseBehavior()
    {
        Ray ray = CommonReference.mainCam.ScreenPointToRay(Input.mousePosition);

        Physics.Raycast(ray, out RaycastHit hit, 100000, 1 << CommonReference.LAYER_GROUND);
        mouse_over = hit.collider == null ? false : hit.collider.gameObject == this.gameObject;

        if (!prev_mouseOver && mouse_over)
        {
            _OnMouseEnter();
        }
        if (prev_mouseOver && !mouse_over)
        {
            _OnMouseExit();
        }
        if (!mouse_drag && mouse_over && Input.GetMouseButtonDown(0))
        {
            mouse_drag = true;
            _OnStartDrag();
        }


        if (mouse_drag)
        {
            _OnMouseDrag();
            if (Input.GetMouseButtonUp(0))
            {
                mouse_drag = false;
                _OnEndDrag();

            }
        }
        prev_mouseOver = mouse_over;
    }

    private void _OnMouseEnter()
    {

    }

    private void _OnMouseExit()
    {

    }
    public void _OnStartDrag()
    {
        drag_start_pos = transform.position;
        float distance_to_screen = CommonReference.mainCam.WorldToScreenPoint(gameObject.transform.position).z;
        moveposition = CommonReference.mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));
        drag_offset = transform.position - moveposition;
        drag_offset.y = 0;
    }

    public void _OnMouseDrag()
    {
        mouseposition = CommonReference.mainCam.ScreenToWorldPoint(Input.mousePosition);
        float distance_to_screen = CommonReference.mainCam.WorldToScreenPoint(gameObject.transform.position).z;
        moveposition = CommonReference.mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));
        Vector3 mpos = new Vector3(moveposition.x, transform.position.y, moveposition.z) + drag_offset;
        Vector3 npos = transform.position;
        bool isLeft = this.transform.position.x < LevelLoader.center.x;
        transform.position = mpos;
    }
    public void _OnEndDrag()
    {
        cpos = LevelLoader.WorldToCellPos(this.transform.position);
        npos = this.transform.position;
        if (!LevelLoader.HasBlockOnCellPos(cpos) && cpos != Vector3.one * -1)
        {
            Debug.Log("Drag success");

            LevelLoader.instance.OnMoveBlock(this, LevelLoader.WorldToCellPos(drag_start_pos), cpos);

            SKUtils.StartProcedure(SKCurve.CubicIn, 0.2f, (f) =>
            {
                transform.position = Vector3.Lerp(npos, cpos, f);


            }, null, gameObject.GetInstanceID() + "drag_success");

        }
        else
        {

            SKUtils.StartProcedure(SKCurve.CubicIn, 0.2f, (f) =>
            {
                transform.position = Vector3.Lerp(npos, drag_start_pos, f);

            }, null, gameObject.GetInstanceID() + "drag_fail");



            Debug.Log("Drag fail");

        }

        Vector3 nscale = transform.localScale;
        SKUtils.StopProcedure(gameObject.GetInstanceID() + "mouse_over");
        SKUtils.StartProcedure(SKCurve.QuadraticOut, 0.1f, (f) =>
        {
            transform.localScale = Vector3.Lerp(LevelLoader.BLOCK_SCALE_MAP, nscale, f);
        }, null, gameObject.GetInstanceID() + "mouse_over");




    }
}


