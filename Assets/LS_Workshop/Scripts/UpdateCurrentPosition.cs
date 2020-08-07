using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateCurrentPosition : MonoBehaviour
{
        public GameObject PlayerController;

        public Text Xvalue;
        public Text Yvalue;
        public Text Zvalue;
        public Text PlotScaleValue;
        public Text PointScaleValue;
        public Text PlotVarianceValue;
        

    void Update()
    {
        GameObject _go = GameObject.Find("LSpaceWorkshop");
        LSpaceController _scr = _go.GetComponent<LSpaceController>();
        float _PlotScale = _scr.PlotScale;
        float _PointScale = _scr.PointScale;
        bool _Variance = _scr.Variance; 

        Vector3 pos = PlayerController.transform.position;
        Xvalue.text = (pos.x  / -_PlotScale).ToString("F4");
        Yvalue.text = (pos.y  / -_PlotScale).ToString("F4");
        Zvalue.text = (pos.z  / -_PlotScale).ToString("F4");

        PlotScaleValue.text = "Plot Scale " + _PlotScale.ToString("F2");
        PointScaleValue.text = "Point Scale " + _PointScale.ToString("F2");
        if (_Variance) {
            PlotVarianceValue.text = "Variance? Y"; }
        else {
            PlotVarianceValue.text = "Variance? N"; }
    }
}
