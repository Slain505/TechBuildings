using System;
using UnityEngine;

namespace DI
{
    public class TestProjectService { }

    public class TestSceneService
    {
        private readonly TestProjectService _projectService;
        
        public TestSceneService(TestProjectService projectService)
        {
            _projectService = projectService;
        }
    }

    public class TestFactory
    {
        public TestObject CreateInstance(string id, int par1)
        {
            return new TestObject(id, par1);
        }
    }
    
    public class TestObject
    {
        private readonly string _id;
        private readonly int _par1;
        
        public TestObject(string id, int par1)
        {
            _id = id;
            _par1 = par1;
        }
    }
    
    public class DIExampleProject : MonoBehaviour
    {
        private void Awake()
        {
            var projectContainer = new DIContainer();
            projectContainer.RegisterSingleton(_ => new TestProjectService());
            projectContainer.RegisterSingleton("Option1", _ => new TestProjectService());
            projectContainer.RegisterSingleton("Option2", _ => new TestProjectService());

            var sceneRoot = FindObjectOfType<DIExampleScene>();
            sceneRoot.Init(projectContainer);
        }
    }
}