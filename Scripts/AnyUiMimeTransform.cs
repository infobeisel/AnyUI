using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AnyUI
{
    public class AnyUiMimeTransform : MonoBehaviour
    {

        public RectTransform UIObjectToMime;
        private RectTransform cvs;
        private RectTransform canvas
        {
            get
            {
                if (cvs == null)
                {
                    var c = UIObjectToMime.GetComponentInParent(typeof(Canvas));
                    cvs = c.GetComponent<RectTransform>();
                }
                return cvs;
            }
        }
        public Vector2 CurrentUVCoordinate
        {
            get
            {
                return new Vector2((UIObjectToMime.anchoredPosition.x + canvas.rect.width * 0.5f) / canvas.rect.width,
                                    (UIObjectToMime.anchoredPosition.y + canvas.rect.height * 0.5f) / canvas.rect.height);
            }
        }
    }
}