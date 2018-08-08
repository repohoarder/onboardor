﻿using GraphQL.Relay.Types;
using GraphQL.Types;
using onboardor.Components.dashboard.onBoardingCreator;
using onboardor.Components.dashboard.onboardingProcess;
using onboardor.Components.select;
using onboardor.Components.shared.form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Onboardor.Components.GraphQl
{
    public class AppMutation : MutationGraphType
    {
        public AppMutation()
        {
            Mutation<AddOnboardingPipelineInput, AddOnboardingPipelinePayload>("addPipeline");
            Mutation<SubscribeMailingListInput, SubscribeMailingListPayload>("subscribeMailingList");
            Mutation<SetOnboardingMembersInput, SetOnboardingMembersPayload>("setOnboardingMembers");
            Mutation<CreateOnboardingProcessInput, CreateOnboardingProcessPayload>("createOnboardingProcess");
        }
    }
}
