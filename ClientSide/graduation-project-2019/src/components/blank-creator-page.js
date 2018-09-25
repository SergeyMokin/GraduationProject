import { Container, Text} from 'native-base';
import React, {Component} from 'react';

export default class BlankCreatorPage extends Component {
  constructor(props){
      super(props);
  }

  render() {
    return (
            <Container>                
                <Text>blank-creator-page</Text>
            </Container>
    );
  }
}