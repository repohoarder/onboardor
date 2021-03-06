﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace onboardor.Components.dashboard
{
    public interface IPipelineService
    {
        IEnumerable<OnboardingPipeline> GetPipelines();
        OnboardingPipeline GetPipeline(int pipelineId);
        void Update(OnboardingPipeline newPipeline);
        void Remove(OnboardingPipeline pipeline);
    }
}
