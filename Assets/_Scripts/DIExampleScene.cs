using UnityEngine;

namespace DI
{
    public class DIExampleScene : MonoBehaviour
    {
        public void Init(DIContainer projectContainer)
        {
            //var serviceWithoutTag = projectContainer.Resolve<MyProjectService>();
            //var serviceWithTag1 = projectContainer.Resolve<MyProjectService>("Option1");
            //var serviceWithTag2 = projectContainer.Resolve<MyProjectService>("Option2");
            
            var sceneContainer = new DIContainer(projectContainer);
            sceneContainer.RegisterSingleton(container => new TestSceneService(container.Resolve<TestProjectService>()));
            sceneContainer.RegisterSingleton(_ => new TestFactory());
            sceneContainer.RegisterInstance(new TestObject("Instance", 0));
            
            var objectsFactory = sceneContainer.Resolve<TestFactory>();

            for (int i = 0; i < 3; i++)
            {
                var id = $"Object{i}";
                var obj = objectsFactory.CreateInstance(id, i);
                Debug.Log($"Created object {id} with factory.\n{obj}");
            }
            
            var instance = sceneContainer.Resolve<TestObject>();
            Debug.Log($"Instance object created in the container.\n{instance}");
        }
    }
}