import { Container, Text, List, Button, ListItem, Segment, InputGroup, Icon, Input, Content, Spinner } from 'native-base';
import React, {Component} from 'react';
import styles from '../styles/mainstyle.js';
import ApiRequests from '../api/index.js';

const buttons = {
  blank: "Blank",
  type: "Type"
};

export default class BlankCreatorPage extends Component {
  constructor(props){
      super(props);

      this.state = {
        isLoading: false,
        enabledButton: buttons.blank,
        typeName: "",
        q1: "",
        q2: "",
        q3: "",
        q4: "",
        q5: "",
        q6: "",
        q7: "",
        inputStyle: {
          color: 'blue'
        },
        messageStyle:{
          color: 'red',
          alignSelf: 'center',
          padding: 20
        }
      }

      this.api = new ApiRequests();
      this.api.setAuthorizationHeader(this.props.userInfo.bearerToken);
      
      this.types = [];
      this.errorMessage = "";
      this.successMessage = "";
      this.typeToAdd = {
        typeName: "",
        questions: []
      }
  }

  async componentWillMount()
  {
    await this.getTypes.call(this);
  }

  async getTypes()
  {
    this.setState({isLoading: true});
    let error = (error) => {
      console.log(error);
      this.setState({isLoading: false});
    };

    let success = (data) => {
        this.types = data;
        this.setState({isLoading: false});
    };

    await this.api.getBlankTypes()
      .then(success.bind(this))
      .catch(error.bind(this));
  }

  async addType()
  {
    this.setState({isLoading: true});
    let error = (error) => {
      this.successMessage = "";
        this.errorMessage = error.message;
        this.setState({
            isLoading: false,
            inputStyle: {
                color: 'red'
            },
            messageStyle:{
              color: 'red',
              alignSelf: 'center',
              padding: 20
            }
        });
    };

    let success = async (data) => {
        this.errorMessage = "";
        this.successMessage = "Success";
        
        this.setState({
            isLoading: false,
            typeName: "",
            q1: "",
            q2: "",
            q3: "",
            q4: "",
            q5: "",
            q6: "",
            q7: "",
            inputStyle: {
                color: 'blue'
            },
            messageStyle:{
              color: 'green',
              alignSelf: 'center',
              padding: 20
            }
          });
        await this.getTypes.call(this);
    };

    this.typeToAdd = {
      typeName: this.state.typeName,
      questions: [this.state.q1, this.state.q2, this.state.q3, this.state.q4, this.state.q5, this.state.q6, this.state.q7]
    }
    await this.api.addBlankType(this.typeToAdd)
      .then(success.bind(this))
      .catch(error.bind(this));
  }

  refresh(){
    this.errorMessage = "";
    this.successMessage = "";
    this.typeToAdd = {
      typeName: "",
      questions: []
    }

    this.setState({
      isLoading: false,
      typeName: "",
      q1: "",
      q2: "",
      q3: "",
      q4: "",
      q5: "",
      q6: "",
      q7: "",
      inputStyle: {
        color: 'blue'
      },
      messageStyle:{
        color: 'red',
        alignSelf: 'center',
        padding: 20
      }
    });
  }

  render() {
    const content = this.state.isLoading ?
    <Content contentContainerStyle={styles.body}>
        <Spinner color="blue" />
    </Content>

    : this.state.enabledButton === buttons.blank ?
    <Button><Text>Blank</Text></Button>

    :
          <List>
            <ListItem>
                <InputGroup>
                    <Icon name="ios-document" style={this.state.inputStyle} />
                    <Input
                        onChangeText={(text) => this.setState({typeName: text})}
                        value={this.state.typeName}
                        placeholder={"Name of type"} />
                </InputGroup>
            </ListItem> 
            <ListItem>
                <InputGroup>
                    <Icon name="ios-help" style={this.state.inputStyle} />
                    <Input
                        onChangeText={(text) => this.setState({q1: text})}
                        value={this.state.q1}
                        placeholder={"Q1"} />
                </InputGroup>
            </ListItem>
            <ListItem>
                <InputGroup>
                    <Icon name="ios-help" style={this.state.inputStyle} />
                    <Input
                        onChangeText={(text) => this.setState({q2: text})}
                        value={this.state.q2}
                        placeholder={"Q2"} />
                </InputGroup>
            </ListItem>
            <ListItem>
                <InputGroup>
                    <Icon name="ios-help" style={this.state.inputStyle} />
                    <Input
                        onChangeText={(text) => this.setState({q3: text})}
                        value={this.state.q3}
                        placeholder={"Q3"} />
                </InputGroup>
            </ListItem>
            <ListItem>
                <InputGroup>
                    <Icon name="ios-help" style={this.state.inputStyle} />
                    <Input
                        onChangeText={(text) => this.setState({q4: text})}
                        value={this.state.q4}
                        placeholder={"Q4"} />
                </InputGroup>
            </ListItem>
            <ListItem>
                <InputGroup>
                    <Icon name="ios-help" style={this.state.inputStyle} />
                    <Input
                        onChangeText={(text) => this.setState({q5: text})}
                        value={this.state.q5}
                        placeholder={"Q5"} />
                </InputGroup>
            </ListItem>
            <ListItem>
                <InputGroup>
                    <Icon name="ios-help" style={this.state.inputStyle} />
                    <Input
                        onChangeText={(text) => this.setState({q6: text})}
                        value={this.state.q6}
                        placeholder={"Q6"} />
                </InputGroup>
            </ListItem>
            <ListItem>
                <InputGroup>
                    <Icon name="ios-help" style={this.state.inputStyle} />
                    <Input
                        onChangeText={(text) => this.setState({q7: text})}
                        value={this.state.q7}
                        placeholder={"Q7"} />
                </InputGroup>
            </ListItem>         
              <Button style={styles.primaryButton} onPress={this.addType.bind(this)}>
                  <Text>Add type</Text>
              </Button>
              <Text style={this.state.messageStyle}>{this.errorMessage !== "" ? this.errorMessage : this.successMessage !== "" ? this.successMessage : ""}</Text>
          </List>                
    ;

    return (
            <Container>
              <Segment style={{backgroundColor:"blue"}}>
                  <Button bordered active={this.state.enabledButton === buttons.blank ? true : false} onPress={() => {this.setState({enabledButton: buttons.blank}); this.refresh();}}>
                        <Text>{buttons.blank}</Text>
                  </Button>              
                  <Button bordered active={this.state.enabledButton === buttons.type ? true : false} onPress={() => {this.setState({enabledButton: buttons.type}); this.refresh();}}>
                        <Text>{buttons.type}</Text>
                  </Button>   
              </Segment>
              <Content>
                {content}
              </Content>
            </Container>
    );
  }
}