public class RendererObject
{
    public int instanceId;
    public string sortingLayerName;
    public int sortingOrder;

    public RendererObject(int instanceId, string layerName, int sortOrder)
    {
        this.instanceId = instanceId;
        sortingLayerName = layerName;
        sortingOrder = sortOrder;
    }
}