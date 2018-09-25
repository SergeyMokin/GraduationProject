import { Container, Text} from 'native-base';
import React, {Component} from 'react';

export default class BlankListPage extends Component {
  constructor(props){
      super(props);
  }

  render() {
    return (
            <Container>                
                <Text>blank-list-page</Text>
            </Container>
    );
  }
}