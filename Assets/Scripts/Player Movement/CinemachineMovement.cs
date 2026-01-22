using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

public class CinemachineMovement : MonoBehaviour
{
    [Header("Assign once (asset, not runtime)")]
    [SerializeField] private InputActionAsset actionsAsset;      // New Controls.inputactions
    [SerializeField] private string actionMapName = "PlayerControls";
    [SerializeField] private string lookActionName = "Look";

    [Header("Controller indices (Controllers.Array.data[index])")]
    [SerializeField] private int orbitXIndex = 0;
    [SerializeField] private int orbitYIndex = 1;

    private MonoBehaviour _axisController;

    private void Awake()
    {
        // auto-find component by name (no drag & drop needed)
        _axisController = GetComponent("CinemachineInputAxisController") as MonoBehaviour;
        if (_axisController == null)
        {
            Debug.LogError("CMInputAxisAutoBind: CinemachineInputAxisController not found on this GameObject.");
            return;
        }

        if (actionsAsset == null)
        {
            Debug.LogError("CMInputAxisAutoBind: Assign Actions Asset (New Controls.inputactions).");
            return;
        }

        var look = actionsAsset.FindAction($"{actionMapName}/{lookActionName}", throwIfNotFound: true);
        look.Enable();

        // runtime reference (nu trebuie “salvat”)
        var lookRef = InputActionReference.Create(look);

        bool xOk = SetControllerInputAction(_axisController, orbitXIndex, lookRef);
        bool yOk = SetControllerInputAction(_axisController, orbitYIndex, lookRef);

        if (!xOk || !yOk)
            Debug.LogError($"CMInputAxisAutoBind: Failed binding. X:{xOk} Y:{yOk} (check indices).");
    }

    private static bool SetControllerInputAction(MonoBehaviour axisController, int controllerIndex, InputActionReference actionRef)
    {
        object mgr = GetFieldOrPropertyValue(axisController, "m_ControllerManager");
        if (mgr == null) return false;

        object controllersObj = GetFieldOrPropertyValue(mgr, "Controllers");
        if (controllersObj == null) return false;

        object controller = GetIndexed(controllersObj, controllerIndex);
        if (controller == null) return false;

        object input = GetFieldOrPropertyValue(controller, "Input");
        if (input == null) return false;

        return SetFieldOrPropertyValue(input, "InputAction", actionRef);
    }

    private static object GetIndexed(object collection, int index)
    {
        if (collection is Array arr)
            return (index >= 0 && index < arr.Length) ? arr.GetValue(index) : null;

        var t = collection.GetType();
        var countProp = t.GetProperty("Count");
        var itemProp = t.GetProperty("Item");
        if (countProp != null && itemProp != null)
        {
            int count = (int)countProp.GetValue(collection);
            return (index >= 0 && index < count) ? itemProp.GetValue(collection, new object[] { index }) : null;
        }
        return null;
    }

    private static object GetFieldOrPropertyValue(object obj, string name)
    {
        if (obj == null) return null;
        var t = obj.GetType();

        var f = t.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (f != null) return f.GetValue(obj);

        var p = t.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (p != null) return p.GetValue(obj);

        return null;
    }

    private static bool SetFieldOrPropertyValue(object obj, string name, object value)
    {
        if (obj == null) return false;
        var t = obj.GetType();

        var f = t.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (f != null) { f.SetValue(obj, value); return true; }

        var p = t.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (p != null && p.CanWrite) { p.SetValue(obj, value); return true; }

        return false;
    }
}
