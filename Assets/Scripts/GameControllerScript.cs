/*
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 * */

using UnityEngine;

public class GameControllerScript : MonoBehaviour
{
    Camera minimap_cam;
    Transform minimap_cam_trf;

    PCGScript PCG;
    LevelBuilder level_builder;
    GameObject arena;

    Level current_level;

    void Start()
    {
        arena = GameObject.Find("Arena");

        PCG = arena.GetComponent<PCGScript>();

        level_builder = arena.GetComponent<LevelBuilder>();
        level_builder.Init();

        minimap_cam = GameObject.Find("MiniMapCamera").GetComponent<Camera>();
        minimap_cam_trf = GameObject.Find("MiniMapCamera").GetComponent<Transform>();
        minimap_cam.orthographic = true;

        current_level = PCG.New();
        UpdateMiniMapCam();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            current_level = PCG.New();
            UpdateMiniMapCam();
        }
    }

    void UpdateMiniMapCam()
    {
        if (current_level == null)
            return;
        minimap_cam_trf.position = new Vector3(current_level.world_width / 2.0f, 300, current_level.world_width / 2.0f);
        minimap_cam_trf.eulerAngles = new Vector3(90, 0, 0);
        minimap_cam.orthographicSize = current_level.world_width / 2.0f;
        // minimap_cam.rect = new Rect(0, 0, current_level.world_width / 2.0f, current_level.world_height / 2.0f);
    }
}
