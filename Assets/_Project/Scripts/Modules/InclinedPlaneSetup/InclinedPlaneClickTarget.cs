using UnityEngine;
using UnityEngine.EventSystems;

namespace OOPLab.Modules.InclinedPlaneSetup
{
    public class InclinedPlaneClickTarget : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private InclinedPlaneController controller;

        public void SetController(InclinedPlaneController targetController)
        {
            controller = targetController;
        }

        private void OnMouseDown()
        {
            OpenMenu();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OpenMenu();
        }

        private void OpenMenu()
        {
            if (controller != null)
            {
                controller.ShowMenu();
            }
        }
    }
}
