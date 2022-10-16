using UnityEngine.UIElements;
 
public class MyImage : Image
{
    public new class UxmlFactory : UxmlFactory<MyImage, Image.UxmlTraits>{}
 
    public MyImage()
    {
       
    }
}