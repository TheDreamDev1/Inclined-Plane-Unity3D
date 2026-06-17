using UnityEngine;
using UnityEngine.UI;

namespace OOPLab.Modules.InclinedPlaneSetup
{
    public class InclinedPlaneController : MonoBehaviour
    {
        [Header("Scene Parts")]
        [SerializeField] private Transform rampPivot;
        [SerializeField] private Transform heightSupport;

        [Header("Menu")]
        [SerializeField] private GameObject controlPanel;
        [SerializeField] private Button closeButton;

        [Header("Angle Controls")]
        [SerializeField] private Slider angleSlider;
        [SerializeField] private InputField angleInput;
        [SerializeField] private Button angleMinusButton;
        [SerializeField] private Button anglePlusButton;
        [SerializeField] private Button resetButton;

        [Header("Readouts")]
        [SerializeField] private Text angleText;
        [SerializeField] private Text heightText;
        [SerializeField] private Text statusText;

        private const float DefaultAngle = 25f;
        private const float RampLength = 4.5f;
        private const float LowerEndHeight = 0.18f;

        private bool isSyncingUi;
        private float currentAngle = DefaultAngle;

        private void Awake()
        {
            SetupControls();
            SetAngle(DefaultAngle);
            HideMenu();
            SetStatus("Ready. Click the plane to edit angle.");
        }

        public void ShowMenu()
        {
            if (controlPanel != null)
            {
                controlPanel.SetActive(true);
            }
        }

        public void HideMenu()
        {
            if (controlPanel != null)
            {
                controlPanel.SetActive(false);
            }
        }

        public void SetAngle(float angleDegrees)
        {
            currentAngle = Mathf.Clamp(angleDegrees, 0f, 45f);

            if (rampPivot != null)
            {
                Vector3 pivotPosition = rampPivot.position;
                pivotPosition.y = LowerEndHeight + Mathf.Sin(currentAngle * Mathf.Deg2Rad) * RampLength;
                rampPivot.position = pivotPosition;
                rampPivot.localRotation = Quaternion.Euler(0f, 0f, -currentAngle);
            }

            if (heightSupport != null && rampPivot != null)
            {
                Vector3 supportPosition = heightSupport.position;
                supportPosition.y = rampPivot.position.y * 0.5f - 0.05f;
                heightSupport.position = supportPosition;

                Vector3 supportScale = heightSupport.localScale;
                supportScale.y = Mathf.Max(0.2f, rampPivot.position.y);
                heightSupport.localScale = supportScale;
            }

            SyncUiValues();
            UpdateReadouts();
            SetStatus("Angle updated.");
        }

        public void ResetPlane()
        {
            SetAngle(DefaultAngle);
            SetStatus("Angle reset.");
        }

        private void SetupControls()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(HideMenu);
            }

            if (angleSlider != null)
            {
                angleSlider.minValue = 0f;
                angleSlider.maxValue = 45f;
                angleSlider.wholeNumbers = false;
                angleSlider.onValueChanged.AddListener(value =>
                {
                    if (!isSyncingUi)
                    {
                        SetAngle(value);
                    }
                });
            }

            if (angleInput != null)
            {
                angleInput.onEndEdit.AddListener(value =>
                {
                    if (float.TryParse(value, out float parsed))
                    {
                        SetAngle(parsed);
                    }
                    else
                    {
                        SyncUiValues();
                    }
                });
            }

            if (angleMinusButton != null)
            {
                angleMinusButton.onClick.AddListener(() => SetAngle(currentAngle - 1f));
            }

            if (anglePlusButton != null)
            {
                anglePlusButton.onClick.AddListener(() => SetAngle(currentAngle + 1f));
            }

            if (resetButton != null)
            {
                resetButton.onClick.AddListener(ResetPlane);
            }
        }

        private void SyncUiValues()
        {
            isSyncingUi = true;

            if (angleSlider != null)
            {
                angleSlider.value = currentAngle;
            }

            if (angleInput != null)
            {
                angleInput.text = currentAngle.ToString("0.0");
            }

            isSyncingUi = false;
        }

        private void UpdateReadouts()
        {
            if (angleText != null)
            {
                angleText.text = $"Angle: {currentAngle:0.0} deg";
            }

            if (heightText != null)
            {
                float height = Mathf.Sin(currentAngle * Mathf.Deg2Rad) * RampLength;
                heightText.text = $"Raised height: {height:0.00} m";
            }
        }

        private void SetStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
        }
    }
}
