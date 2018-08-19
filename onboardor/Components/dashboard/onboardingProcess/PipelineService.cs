﻿using Microsoft.EntityFrameworkCore;
using Onboardor.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace onboardor.Components.dashboard
{
    public class PipelineService : IPipelineService
    {
        private IRepository<OnboardingPipeline> _repository;

        public PipelineService(IRepository<OnboardingPipeline> repository)
        {
            _repository = repository;
        }

        public OnboardingPipeline GetPipeline(int pipelineId)
        {
            return _repository.GetAll().Include(x => x.OnboardingSteps).Single(x => x.Id == pipelineId);
        }

        public void Update(OnboardingPipeline newPipeline)
        {
            _repository.Update(newPipeline);
        }
    }
}
