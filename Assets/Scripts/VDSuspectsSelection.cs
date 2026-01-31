using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VDSuspectsSelection : Singleton<VDSuspectsSelection>
{
    [SerializeField] Image[] thumbnailsImages;
    [SerializeField] Transform selectedFrame;
    [SerializeField] GameObject loadPicture;
    [SerializeField] Image pictureImage;
    [SerializeField] float loadPictureTime = 1;
    int selectedIdx = -1;

    protected override void Awake()
    {
        base.Awake();
        
        selectedFrame.gameObject.SetActive(false);
        loadPicture.SetActive(false);
    }
    
    void OnDisable()
    {
        SelectIdx(-1);
    }
    
    public void SelectIdx(int idx)
    {
        selectedIdx = idx;

        if (idx == -1)
        {
            selectedFrame.gameObject.SetActive(false);

            pictureImage.sprite = null;
            pictureImage.enabled = false;
            loadPicture.SetActive(false);

            CancelInvoke("PictureLoaded");
            return;
        }

        selectedFrame.gameObject.SetActive(true);
        selectedFrame.position = thumbnailsImages[selectedIdx].transform.position;

        LoadPicture(VigilenceDirect.Instance.crtLevel.suspects[idx].picture);
    }

    public void Swip(bool left) => SelectIdx((left ? selectedIdx - 1 + thumbnailsImages.Length : selectedIdx + 1) % thumbnailsImages.Length);

    void LoadPicture(Sprite picture)
    {
        pictureImage.sprite = picture;
        pictureImage.enabled = false;
        loadPicture.SetActive(true);
        CancelInvoke("PictureLoaded");
        Invoke("PictureLoaded", loadPictureTime);
    }

    void PictureLoaded()
    {
        loadPicture.SetActive(false);
        pictureImage.enabled = true;
    }
}
