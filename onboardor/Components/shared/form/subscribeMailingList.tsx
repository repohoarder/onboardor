import { Box, Flex } from "grid-styled";
import React from "react";
import { Absolute, Relative, Label, Input } from "rebass";
import { Field, InjectedFormProps, change } from "redux-form";
import createFieldValidator from "../inputs/createFieldValidator";
import EmailIcon from "../../../wwwroot/assets/email-green.svg";
import { withTheme } from "styled-components";
import { IStyleProps } from "../../types";
import Button from "../button/button";
import ReCAPTCHA from "react-google-recaptcha";
import { IMutationInput } from "./subscribeMailingListMutation";

export interface IProps extends InjectedFormProps, IStyleProps {
  onSubmit: ({}) => void;
  setRecaptcha: () => ReCAPTCHA;
  recaptcha: ReCAPTCHA;
}

const SubscribeMailingList = ({
  handleSubmit,
  onSubmit,
  theme,
  setRecaptcha,
  recaptcha,
  error,
}: IProps) => (
  <Box mx="auto">
    <form onSubmit={(e) => {
      e.preventDefault();
      recaptcha && recaptcha.execute()}
    } action="">
      <Flex justifyContent="center">
        <Relative>
          <Field
            component={Input}
            px={63}
            py={27}
            fontSize={17}
            bg="white"
            id="email"
            name="email"
            placeholder="Enter your email"
            type="email"
            validate={createFieldValidator(["required", "email"])}
          />
          <Absolute top="50%" left={30} style={{ transform: "translateY(-50%)" }}>
            <Label htmlFor="email" mb={0}>
              <EmailIcon />
            </Label>
          </Absolute>
        </Relative>
        <Button
          ml={15}
          fontSize={17}
          px={[20, 76]}
          style={{ textTransform: "uppercase", whiteSpace: "nowrap" }}
        >
          Join List
        </Button>
      </Flex>
      <ReCAPTCHA
        ref={setRecaptcha}
        size="invisible"
        sitekey={process.env.RECAPTCHA_SITE_KEY as string}
        onChange={(recaptcha) => {
          handleSubmit((props) => onSubmit({ ...props, recaptcha }))()
        }}
      />
    </form>
  </Box>
);

export default withTheme(SubscribeMailingList);
